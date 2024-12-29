namespace Firework.Dto.Results;

public class GetLoadResult
{
    public float CpuLoad { get; set; }
    public float GpuLoad { get; set; }
    public float RamLoad { get; set; }
}