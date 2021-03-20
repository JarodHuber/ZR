public enum GrabState { UnGrabbed, GrabR, GrabL }

public enum SpawnStage { Wait, Start, Play }

public enum ButtonType { SceneChanger }

public enum ObjectReferenceType { Enemy, Object, Player, Projectile }

public enum HandSide { Left, Right }

public enum GrabType { obj, ledge, blank }

public enum ButtonPushDirection { Down, Up, Backward, Forward, Left, Right, Special }

public enum ObjectType { None = 0, Gun = 1<<0, Pistol = 1<<1, Grenade = 1<<2 }

[System.Flags]
public enum ObjectsToHolster { None = 0, Gun = 1 << 0, Pistol = 1 << 1, Grenade = 1 << 2 }