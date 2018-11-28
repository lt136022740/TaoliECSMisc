using Unity.Entities;
using Unity.Mathematics;

public struct FormationData : IComponentData
{
    public int width;
    public int height;
    public float3 formationPosition;

    private void start()
    {

    }

    public int count()
    {
        return width * height;
    }

    public void init(int width, int height)
    {
        this.width = width;
        this.height = height;

    }
}
public class FormationComponent : ComponentDataWrapper<FormationData> { }