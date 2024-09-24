using Godot;
using System;

public partial class BgMusic : Node
{
    public void play()
    {
        GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
    }

}
