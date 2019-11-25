﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IL2CDR.Model
{
	public class DServerManager : IStopStart
	{
		private readonly object lockDservers = new object();
		private readonly ActionManager actionManager;
		private readonly ProcessMonitor dserverProcMonitor;

		public DServerManager()
		{
			this.dserverProcMonitor = new ProcessMonitor("DServer.exe");
			this.Servers = new ObservableCollection<Server>();
			this.dserverProcMonitor.AddProcess = (process) => { this.AddServer(this.GetServer(process)); };
			this.dserverProcMonitor.RemoveProcess = this.RemoveServer;
			this.dserverProcMonitor.Start();
			this.actionManager = (Application.Current as App)
				.Return(x => x.ActionManager, new ActionManager(new ScriptManager()));
		}

		public ObservableCollection<Server> Servers { get; set; }

		public void Start()
		{
			this.dserverProcMonitor.Start();
			foreach (var server in this.dserverProcMonitor.RunningProcesses.ToList()) {
				this.AddServer(this.GetServer(server));
			}
		}

		private Server GetServer(ProcessItem process)
		{
			var baseDir = Directory.GetParent(Directory.GetParent(process.Path).FullName);
			var config = new IL2StartupConfig(string.Concat(baseDir, @"\data\startup.cfg"));
			var rcon = new RconConnection(config);
			var server = new Server(rcon, process);
			return new Server(rcon, process);
		}

		private void RemoveServer(int processId)
		{
			lock (this.lockDservers) {
				var server =
					this.Servers.FirstOrDefault(ds => ds?.Process != null && ds.Process.Id == processId);
				if (server != null) {
					server.MissionLogService.Stop();
					Util.Try(() => server.Rcon.Stop());
					UI.Dispatch(() => {
						this.Servers.Remove(server);
						this.dserverProcMonitor.Remove(processId);
					});
				}
			}
		}

		private void AddServer(Server server)
		{
			var existingDServer = this.Servers.FirstOrDefault(s =>
				s.Rcon.Config.GameRootFolder.Equals(server.Rcon.Config.GameRootFolder));
			if (existingDServer != null) {
				var process = Process.GetProcessById((int) existingDServer.Process.Id);
				//Previous DServer process didn't exit correctly
				if (process != null) {
					process.Kill();
					this.RemoveServer(existingDServer.Process.Id);
					this.actionManager.RunServerStopScript(server);
				}
			}

			UI.Dispatch(() => {
				lock (this.lockDservers) {
					this.Servers.Add(server);
					this.actionManager.RunServerStartScripts(server);
				}
			});
			Task.Factory.StartNew((obj) => {
				if (!(obj is Server srv)) {
					return;
				}

				srv.IsConfigSet = srv.Rcon.Config.IsConfigReady;
				srv.Login();
			}, server, TaskCreationOptions.LongRunning).ContinueWith((task, obj) => {
				if (!(obj is Server srv)) {
					return;
				}

				srv.ServerId = GuidUtility.Create(GuidUtility.IsoOidNamespace, srv.Name);
				Log.WriteInfo("Start server monitor for server {0}", srv.Name);
				srv.MissionLogService.Start();
				srv.IsRconConnected = true;
			}, server);
		}

		public Server this[string key] => this.GetServerById(key);

		public Server GetServerById(string id)
		{
			return this.Servers.FirstOrDefault(s => s != null &&
													s.ServerId != Guid.Empty &&
													s.ServerId.ToString().Equals(id,
														StringComparison.InvariantCultureIgnoreCase));
		}

		public void Stop()
		{
			this.dserverProcMonitor.Stop();
			foreach (var server in this.Servers) {
				Util.Try(() => server.Rcon.Stop());
				server.MissionLogService.Stop();
			}
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}
	}
}