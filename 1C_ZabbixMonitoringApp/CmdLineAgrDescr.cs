
namespace _1C_ZabbixMonitoringApp
{
    class CmdLineAgrDescr
    {
        public string Name { get; set; }
        public bool HasParameter { get; set; }
        public string Descr { get; set; }
        public string Value { get; set; } = "";

        public override string ToString()
        {
            return $"\t{Name}\t\t{Descr}";
        }
    }
}
