using System;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class BeastFangs : BaseDefaultEquipmentMutation
    {

        public GameObject BeakObject;

        public string BodyPartType = "Face";

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BeastFangs obj = base.DeepCopy(Parent, MapInv) as BeastFangs;
            obj.BeakObject = null;
            return obj;
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool GeneratesEquipment()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "Your face bears a sharp set of carnivorous fangs.";
        }
        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if (!TryGetRegisteredSlot(body, BodyPartType, out var Part))
            {
                Part = body.GetFirstPart(BodyPartType);
                if (Part != null)
                {
                    RegisterSlot(BodyPartType, Part);
                }
            }
            if (Part != null)
            {
                BeakObject = GameObjectFactory.Factory.CreateObject(Variant ?? "BeastFangs");
                MeleeWeapon part = BeakObject.GetPart<MeleeWeapon>();
                Armor part2 = BeakObject.GetPart<Armor>();
                part.Skill = "ShortBlades";
                part.BaseDamage = "1";
                part.Slot = Part.Type;
                part2.WornOn = Part.Type;
                part2.AV = 0;
                Part.DefaultBehavior = BeakObject;
                Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "BeastFangs");
                ResetDisplayName();
            }
            base.OnRegenerateDefaultEquipment(body);
        }
        public override bool Unmutate(GameObject GO)
        {
            base.StatShifter.RemoveStatShifts(ParentObject);
            CleanUpMutationEquipment(GO, ref BeakObject);
            return base.Unmutate(GO);
        }
    }
}