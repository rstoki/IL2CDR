<?php
require_once 'Model/MissionEvent.php';
/**
 * IndexController short summary.
 *
 * IndexController description.
 *
 * @version 1.0
 * @author Anton
 */
class IndexController
{
    private $templates;
    function __construct( League\Plates\Engine $templates)
    {
    	$this->templates = $templates;
    }
    
    public function GetIndex( )
    {
        return $this->templates->render('kd');
    }
    public function GetKD( )
    {
        return $this->templates->render('kd');
    }
    public function GetSnipers( )
    {
        return $this->templates->render('snipers');
    }
    public function GetSurvivors( )
    {
        return $this->templates->render('survivors');
    }
    
    public function PostEvent($json)               
    {
        if( !isset($_COOKIE['srvtoken']) && empty($_COOKIE['srvtoken'])  )
            return "TOKEN FAIL";
        if( isset($json)  )
        {                    
            $event = new MissionEvent( $json );        
            if( $event )
            {
                if( $event->SaveToDB() )
                {
                    return "OK";
                }
                else 
                {
                    return "FAIL";
                }
            }        
        }
        return "UNKNOWN EVENT";          
        
    }
    
    
}

function activeIfMatch($requestUri)
{
    $current_file_name = basename($_SERVER['REQUEST_URI'], ".php");

    if ($current_file_name == $requestUri)
        echo 'class="active"';
}

?>