using System;
using System.Collections.Generic;
using System.Text;
using ConsoleLib.Console;
using XRL.Rules;
using XRL.UI;
using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class BeastTail : BaseDefaultEquipmentMutation
    {

        public GameObject TailObject;
        public string ManagerID => ParentObject.ID + "::BeastTail";

        public BeastTail()
        {
        }

        public void SetColor(string tile, string detail)
        {
            TailObject.Render.TileColor = tile;
            TailObject.Render.DetailColor = detail;
        }

        public override string GetDescription()
        {
            if (!Variant.IsNullOrEmpty())
            {
                return "Your rear bears a " + GetVariantName().ToLowerInvariant();
            }
            return "Your rear bears a tail.";
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BeastTail obj = base.DeepCopy(Parent, MapInv) as BeastTail;
            obj.TailObject = null;
            return obj;
        }
        public override bool GeneratesEquipment()
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != PooledEvent<CommandEvent>.ID && ID != AIGetOffensiveAbilityListEvent.ID)
            {
                return ID == SingletonEvent<BeforeAbilityManagerOpenEvent>.ID;
            }
            return true;
        }
        public override bool CanLevel()
        {
            return false;
        }


        public override IRenderable GetIcon()
        {
            if (!MutationFactory.TryGetMutationEntry(this, out var Entry))
            {
                return null;
            }
            return Entry.GetRenderable();
        }
        public override void OnRegenerateDefaultEquipment(Body body)
        {
            BodyPart partByManager = body.GetPartByManager(ManagerID);
            if (partByManager != null)
            {
                AddTailTo(partByManager);
            }
        }

        public static bool IsUnmanagedPart(BodyPart Part)
        {
            return Part.Manager.IsNullOrEmpty();
        }

        public static BodyPart AddTail(GameObject Object, string ManagerID, bool UseUnmanaged = false, bool DoUpdate = true)
        {
            BodyPart bodyPart = Object?.Body?.GetBody();
            if (bodyPart == null)
            {
                return null;
            }
            if (UseUnmanaged)
            {
                BodyPart firstPart = bodyPart.GetFirstPart("BeastTail", IsUnmanagedPart);
                if (firstPart != null)
                {
                    firstPart.Manager = ManagerID;
                    return firstPart;
                }
            }
            Object.WantToReequip();
            BodyPart firstAttachedPart = bodyPart.GetFirstAttachedPart("BeastTail", 0, Object.Body, EvenIfDismembered: true);
            int? num;
            bool doUpdate;
            if (firstAttachedPart != null)
            {
                firstAttachedPart.ChangeLaterality(2);
                num = bodyPart.Category;
                int? category = num;
                doUpdate = DoUpdate;
                return bodyPart.AddPartAt(firstAttachedPart, "Tail", 1, null, null, null, null, ManagerID, category, null, null, null, null, null, null, null, null, null, null, null, null, doUpdate);
            }
            num = bodyPart.Category;
            int? category2 = num;
            string[] orInsertBefore = new string[3] { "Roots", "Thrown Weapon", "Floating Nearby" };
            doUpdate = DoUpdate;
            return bodyPart.AddPartAt("Tail", 0, null, null, null, null, ManagerID, category2, null, null, null, null, null, null, null, null, null, null, null, null, "Feet", orInsertBefore, doUpdate);
        }

        public static void RemoveTail(GameObject Object, string ManagerID)
        {
            BodyPart bodyPartByManager = Object.GetBodyPartByManager(ManagerID);
            if (bodyPartByManager != null)
            {
                int laterality = bodyPartByManager.Laterality;
                int laterality2 = ((bodyPartByManager.Laterality != 1) ? 1 : 2);
                Object.RemoveBodyPartsByManager(ManagerID, EvenIfDismembered: true);
                bodyPartByManager = Object.GetFirstBodyPart("BeastTail", laterality2);
                if (bodyPartByManager != null && bodyPartByManager.IsLateralityConsistent() && !Object.HasBodyPart("BeastTail", laterality))
                {
                    bodyPartByManager.ChangeLaterality(0);
                }
                Object.WantToReequip();
            }
        }

        public void AddTailTo(BodyPart Limb)
        {
            if (TailObject == null)
            {
                TailObject = GameObject.Create(Variant ?? "BeastTail");
            }
            int level = base.Level;
            bool flag = TailObject.EquipAsDefaultBehavior();
            MeleeWeapon part = TailObject.GetPart<MeleeWeapon>();
            if (part != null)
            {
                part.Slot = Limb.Type;
            }
            if (flag && Limb.DefaultBehavior != null)
            {
                if (Limb.DefaultBehavior == TailObject)
                {
                    return;
                }
                Limb.DefaultBehavior = null;
            }
            if (!flag && Limb.Equipped != null)
            {
                if (Limb.Equipped == TailObject)
                {
                    return;
                }
                if (Limb.Equipped.CanBeUnequipped(null, null, Forced: false, SemiForced: true))
                {
                    Limb.ForceUnequip(Silent: true);
                }
            }
            if (!Limb.Equip(TailObject, 0, Silent: true, ForDeepCopy: false, Forced: false, SemiForced: true))
            {
                CleanUpMutationEquipment(ParentObject, ref TailObject);
            }
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            BodyPart bodyPart = AddTail(GO, ManagerID, UseUnmanaged: true);
            if (bodyPart != null)
            {
                AddTailTo(bodyPart);
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            CleanUpMutationEquipment(GO, ref TailObject);
            RemoveTail(GO, ManagerID);
            return base.Unmutate(GO);
        }
    }
}