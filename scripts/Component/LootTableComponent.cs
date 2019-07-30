using GameFeel.Component.Subcomponent;
using Godot;
using GodotTools.Extension;
using GodotTools.Logic;

namespace GameFeel.Component
{
    [Tool]
    public class LootTableComponent : Node
    {
        private LootTable<LootTableItem> _lootTable = new LootTable<LootTableItem>();

        public override void _Ready()
        {
            foreach (var child in GetChildren())
            {
                if (child is LootTableItem li)
                {
                    _lootTable.AddItem(li, li.Weight);
                }
            }
            GetOwner()?.GetFirstNodeOfType<HealthComponent>()?.Connect(nameof(HealthComponent.HealthDepleted), this, nameof(OnHealthDepleted));
        }

        public override string _GetConfigurationWarning()
        {
            var valid = true;
            foreach (var child in GetChildren())
            {
                if (!(child is LootTableItem))
                {
                    valid = false;
                    break;
                }
            }

            return valid ? string.Empty : "Must contain only children of type " + nameof(LootTableItem);
        }

        private void SpawnItem()
        {
            var toSpawn = _lootTable.PickItem();
            if (GetOwner() is Node2D node)
            {
                var scene = toSpawn.Scene?.Instance() as Node2D;
                if (scene != null)
                {
                    //TODO: I DON'T WANT TO DO THIS
                    GameZone.EntitiesLayer.CallDeferred("add_child", scene);
                    // GameZone.EntitiesLayer.AddChild(scene);
                    scene.GlobalPosition = node.GlobalPosition;
                }
            }
        }

        private void OnHealthDepleted()
        {
            SpawnItem();
        }
    }
}