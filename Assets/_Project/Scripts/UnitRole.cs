using System;
using System.Collections.Generic;

namespace AnimalAutoBattle.Units
{
    public enum UnitRole
    {
        Vanguard,
        Bruiser,
        Enforcer,
        Bulwark,
        Marksman,
        Voidcaller,
        Blaster,
        Healer,
        Augmenter,
        Controller,
        Wardkeeper,
        Infiltrator,
        Bleeder,
        Engineer,
        Disruptor,
        Guardian,
        Volleyer,
        Snarecaster,
        Harrier,
        Invoker,
        Channeler,
        Venomancer,
        Lurker,
        Striker,
        Operator,
        Augmentalist,
        Supportbot
    }

    public enum UnitClass
    {
        Melee,
        Ranged,
        Magic,
        Support,
        Flank,
        Tech
    }

    public static class UnitRoleExtensions
    {
        private static readonly Dictionary<UnitRole, UnitClass> RoleToClassMap;

        static UnitRoleExtensions()
        {
            RoleToClassMap = new Dictionary<UnitRole, UnitClass>();

            AddRoles(UnitClass.Melee, UnitRole.Vanguard, UnitRole.Bruiser, UnitRole.Guardian, UnitRole.Enforcer, UnitRole.Bulwark);
            AddRoles(UnitClass.Ranged, UnitRole.Marksman, UnitRole.Volleyer, UnitRole.Snarecaster, UnitRole.Harrier);
            AddRoles(UnitClass.Magic, UnitRole.Invoker, UnitRole.Channeler, UnitRole.Venomancer, UnitRole.Voidcaller, UnitRole.Blaster);
            AddRoles(UnitClass.Tech, UnitRole.Engineer, UnitRole.Augmentalist, UnitRole.Disruptor, UnitRole.Supportbot);
            AddRoles(UnitClass.Flank, UnitRole.Striker, UnitRole.Infiltrator, UnitRole.Bleeder, UnitRole.Lurker);
            AddRoles(UnitClass.Support, UnitRole.Healer, UnitRole.Augmenter, UnitRole.Controller, UnitRole.Wardkeeper);
        }

        private static void AddRoles(UnitClass unitClass, params UnitRole[] roles)
        {
            foreach (var role in roles)
            {
                RoleToClassMap[role] = unitClass;
            }
        }

        public static UnitClass GetClass(this UnitRole role)
        {
            if (RoleToClassMap.TryGetValue(role, out var unitClass))
                return unitClass;

            throw new ArgumentOutOfRangeException(nameof(role), $"Unhandled role: {role}");
        }
    }
}