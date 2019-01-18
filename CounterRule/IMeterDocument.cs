namespace CounterRule
{
    public interface IMeterDocument
    {
         long Start { get; set; }
         long EllepsitTime { get; set; }
         byte[] RequestData { get; set; }
         byte[] ResponseData { get; set; }
         int ResponseStutus { get; set; }
         string Ip { get; set; }
         string UserName { get; set; }
    }
    
}
