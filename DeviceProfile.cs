using System.Runtime.Serialization;

namespace MonitorControl
{
    #region JSON datas
    [DataContract]
    internal class DeviceProfile
    {
        [DataMember(Order = 1)]
        public uint Brightness { set; get; }

        [DataMember(Order = 2)]
        public uint Contrast { set; get; }

        [DataMember(Order = 3)]
        public uint Red { set; get; }

        [DataMember(Order = 4)]
        public uint Green { set; get; }

        [DataMember(Order = 5)]
        public uint Blue { set; get; }

        public (uint, uint, uint, uint, uint) Values
        {
            get => (Brightness, Contrast, Red, Green, Blue);
            set => (Brightness, Contrast, Red, Green, Blue) = value;
        }
    }
    #endregion

}
