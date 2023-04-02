using Godot;
using System;
using Utils;

public class ComboTester : Node
{
    private BtnComboDetector bcd = new BtnComboDetector("ui_left", "ui_right");
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var status = bcd.GetComboStatus();
        if (status.IsDetected)
            GD.Print(status.IsSuccessful);
    }
}
