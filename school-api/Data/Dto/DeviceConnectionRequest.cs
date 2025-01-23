namespace school_api.Data.Dto
{
    public class DeviceConnectionRequest
    {
        public string event_sub_group { get; set; }
        public string event_group { get; set; }
        public string wsop_id { get; set; }
        public int event_sequence_number { get; set; }
        public string platform_type { get; set; }
        public string wsop_session_id { get; set; }
        public string login_mode { get; set; }
        public string app_version { get; set; }
        public string setup_environment { get; set; }
        public string os { get; set; }
        public string model_name { get; set; }
        public int device_performance { get; set; }
        public string network_type { get; set; }
        public int network_latency { get; set; }
        public string event_stream_session_id { get; set; }
        public string client_install_guid { get; set; }
        public string wsop_client_id { get; set; }
        public string casino_id { get; set; }
        public string msg_id { get; set; }
        public object server_timestamp { get; set; }
        public int time_elapsed_since_start { get; set; }
        public int session_count { get; set; }
        public int trs_id { get; set; }
        public int clubs_rank { get; set; }
        public string test_group { get; set; }
        public bool test_user { get; set; }
        public string event_category { get; set; }
        public string action { get; set; }
        public string label { get; set; }
        public double value { get; set; }
        public string context { get; set; }
        public string event_type { get; set; }
        public string funnel { get; set; }
        public string step { get; set; }
        public string description { get; set; }
        public string funnel_session_id { get; set; }
        public string screen_name { get; set; }
    }
}
