namespace TechnicalInfo.Domain.Models
{
    public enum MonitorConnectionPorts
    {
        D3DKMDT_VOT_UNINITIALIZED = -2,
        D3DKMDT_VOT_OTHER = -1,
        D3DKMDT_VOT_HD15 = 0,
        D3DKMDT_VOT_SVIDEO = 1,
        D3DKMDT_VOT_COMPOSITE_VIDEO = 2,
        D3DKMDT_VOT_COMPONENT_VIDEO = 3,
        D3DKMDT_VOT_DVI = 4,
        D3DKMDT_VOT_HDMI = 5,
        D3DKMDT_VOT_LVDS = 6,
        D3DKMDT_VOT_D_JPN = 8,
        D3DKMDT_VOT_SDI = 9,
        D3DKMDT_VOT_DISPLAYPORT_EXTERNAL = 10,
        D3DKMDT_VOT_DISPLAYPORT_EMBEDDED = 11,
        D3DKMDT_VOT_UDI_EXTERNAL = 12,
        D3DKMDT_VOT_UDI_EMBEDDED = 13,
        D3DKMDT_VOT_SDTVDONGLE = 14,
        D3DKMDT_VOT_MIRACAST = 15
    }
    
    public class MonitorModel
    {
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Instancename { get; set; }
        public MonitorConnectionPorts MonitorConnectionPort { get; set; }
        public string MonitorConnectionPortName { get; set; }
    }
}
