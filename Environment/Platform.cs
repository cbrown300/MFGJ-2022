using Godot;
using Godot.Collections;
public class Platform : Node2D
{
    private Array moveLocations;
    private Tween tween;    //used for the area between move locations
    private KinematicBody2D platform;
    private int index = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        moveLocations = GetNode<Node>("MovementLocations").GetChildren();
        tween = GetNode<Tween>("KinematicBody2D/Tween");
        platform = GetNode<KinematicBody2D>("KinematicBody2D");
        _on_Tween_tween_completed(null, null);
    }

    //Manages moving the platform between move locations
    private void _on_Tween_tween_completed(object obj, NodePath path){
        index += 1;
        if(index > moveLocations.Count - 1){
            index = 0;
        }
        Position2D node = moveLocations[index] as Position2D;
        tween.InterpolateProperty(platform, "position", platform.Position, node.Position, node.Position.DistanceTo(platform.Position) / 30, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        tween.Start();
    }
}
