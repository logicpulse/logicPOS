namespace LogicPOS.Api
{
    public struct ApiSettings
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Scheme { get; set; }

        public string BaseAddress => $"{Scheme}://{Ip}:{Port}/";
    }
}
