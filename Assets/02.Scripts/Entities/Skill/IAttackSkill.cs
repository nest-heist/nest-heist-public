using System.Collections.Generic;
using UnityEngine;

public interface IProjectileSkill
{
    GameObject ProjectileObject { get; set; }
    float ProjectileSpeed { get; set; }

    void ApplyProjectileEffect(Vector2 targetPos);
}

public interface IAreaSkill
{
    GameObject AOEObject { get; set; }
    float Radius { get; set; }

    void ApplyAreaEffect(Vector2 targetPos);
}

public interface IDoTSkill
{
    float TickInterval { get; set; }
    float Duration { get; set; }
    void ApplyDoTDamage(Vector2 targetPos);
}

public interface IBoxAreaSkill
{
    GameObject AOEObject { get; set; }
    Vector2 BoxDimensions { get; set; }
    void ApplyBoxAreaEffect(Vector2 targetPos);
}

public interface ITargetSkill
{
    void ApplyTargetEffect(Vector2 targetPos);
}
