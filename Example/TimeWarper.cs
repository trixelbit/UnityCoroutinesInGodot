using Godot;
using System;

public class TimeWarper : Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private HSlider _slider;
    private RichTextLabel _label; 
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _slider = GetNode<HSlider>("HSlider");
        _label = GetNode<RichTextLabel>("RichTextLabel");
    }

    public override void _Process(float delta)
    {
        Engine.TimeScale = (float)_slider.Value;

        _label.Text = $"TimeScale: {Engine.TimeScale}";
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
