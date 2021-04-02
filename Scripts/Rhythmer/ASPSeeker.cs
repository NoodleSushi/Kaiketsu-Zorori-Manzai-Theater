using Godot;
using System;

[Tool]
public class ASPSeeker : AnimationNode
{
    public override string GetCaption() => "ASPSeeker";

    public ASPSeeker()
    {
        AddInput("in");
        AddInput("shot");
    }

    public override void Process(float time, bool seek)
    {
        BlendInput(0, time, false, 1.0f);
        BlendInput(1, time, true, 1.0f);
    }
}
