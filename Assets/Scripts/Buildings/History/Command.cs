using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Command
{
    public abstract void Execute();
    public abstract void Unexecute();
}

public class AddBuildingCommand : Command
{
    private Vector3Int position;
    private TileBase tile;
    private Tilemap buildingsTilemap;
    private Building building;
    private GameObject instance;
    private BuildingManager buildingManager;
    private int id;

    public AddBuildingCommand(Vector3Int position, Building building, GameObject instance, BuildingManager buildingManager, int id, Tilemap buildingsTilemap)
    {
        this.position = position;
        this.tile = building.GetState();
        this.buildingsTilemap = buildingsTilemap;
        this.building = building;
        this.instance = instance;
        this.buildingManager = buildingManager;
        this.id = id;
    }



    public override void Execute()
    {
        buildingsTilemap.SetTile(position, tile);
        buildingManager.AddBuilding(position, building, instance, id);
    }

    public override void Unexecute()
    {
        buildingsTilemap.SetTile(position, null);
        buildingManager.RemoveBuilding(position);
        Object.Destroy(instance);
    }
}

public class RemoveBuildingCommand : Command
{
    private Vector3Int position;
    private BuildingManager.BuildingInfo buildingInfo;
    private Tilemap buildingsTilemap;
    private BuildingManager buildingManager;

    public RemoveBuildingCommand(Vector3Int position, BuildingManager.BuildingInfo buildingInfo, Tilemap buildingsTilemap, BuildingManager buildingManager)
    {
        this.position = position;
        this.buildingInfo = buildingInfo;
        this.buildingsTilemap = buildingsTilemap;
        this.buildingManager = buildingManager;
    }

    public override void Execute()
    {
        buildingsTilemap.SetTile(position, null);
        buildingManager.RemoveBuilding(position);
        Object.Destroy(buildingInfo.Instance);
    }

    public override void Unexecute()
    {
        TileBase activeTile = buildingInfo.PrefabType.GetState();
        buildingsTilemap.SetTile(position, activeTile);
        Building newBuilding = Object.Instantiate(buildingInfo.PrefabType, position, Quaternion.identity);
        buildingManager.AddBuilding(position, buildingInfo.PrefabType, newBuilding.gameObject, buildingInfo.Id);
    }
}

public class BatchCommand : Command
{
    private List<Command> commands = new List<Command>();

    public void AddCommand(Command command)
    {
        commands.Add(command);
    }

    public override void Execute()
    {
        foreach (Command command in commands)
        {
            command.Execute();
        }
    }

    public override void Unexecute()
    {
        for (int i = commands.Count - 1; i >= 0; i--)
        {
            commands[i].Unexecute();
        }
    }
}
