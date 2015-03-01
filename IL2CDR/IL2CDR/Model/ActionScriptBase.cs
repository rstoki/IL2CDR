﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public class ActionScriptBase : IActionScript, IScriptConfig
    {
        virtual public void OnVersion(MissionLogEventVersion data){}

        virtual public void OnInfluenceAreaBoundary(MissionLogEventInfluenceAreaBoundary data){}

        virtual public void OnInfluenceAreaInfo(MissionLogEventInfluenceAreaInfo data){}

        virtual public void OnGameObjectSpawn(MissionLogEventGameObjectSpawn data){}

        virtual public void OnGroupInitInfo(MissionLogEventGroupInitInfo data){}

        virtual public void OnPlayerPlaneSpawn(MissionLogEventPlaneSpawn data){}

        virtual public void OnAirfieldInfo(MissionLogEventAirfieldInfo data){}

        virtual public void OnObjectiveCompleted(MissionLogEventObjectiveCompleted data){}

        virtual public void OnMissionEnd(MissionLogEventMissionEnd data){}

        virtual public void OnLanding(MissionLogEventLanding data){}

        virtual public void OnTakeOff(MissionLogEventTakeOff data){}

        virtual public void OnPlayerMissionEnd(MissionLogEventPlayerAmmo data){}

        virtual public void OnKill(MissionLogEventKill data){}

        virtual public void OnDamage(MissionLogEventDamage data){}

        virtual public void OnHit(MissionLogEventHit data){}

        virtual public void OnMissionStart(MissionLogEventStart data){}

        virtual public void OnUserId(MissionJoin data){}

        virtual public void OnOther(object data){}

        virtual public ScriptConfig DefaultConfig
        {
            get
            {
                return new ScriptConfig()
                {
                    Enabled = false,
                    Title = "Example script",
                    Description = "This script does nothing",
                };
            }
        }

        virtual public ScriptConfig Config
        {
            get;
            set;
        }
    }
}
