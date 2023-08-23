

/// <summary>
/// An enumeration of pooled object names. 
/// Make sure the enum values mirror prefab paths, and their sorting too.
/// i.e. all prefabs that will be loaded as terrain impacts must be in the folder
/// "Resources/Prefabs/Effects/Impacts/Terrain" and the enum value has to 
/// match this path after Prefabs/ , with / chars replaced with _
/// </summary>
public enum PooledObjectType 
{
    Obstacles_HexagonObstacle1,
    Obstacles_HexagonObstacle2,
    Obstacles_HexagonObstacle3,
    Obstacles_TriangleObstacle1,
    Obstacles_TriangleObstacle2,
    Obstacles_TriangleObstacle3
}
