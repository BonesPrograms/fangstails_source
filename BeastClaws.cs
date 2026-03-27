using System;
using System.Collections.Generic;
using XRL.World.Anatomy;


namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class BeastClaws : BaseDefaultEquipmentMutation
    {
        public GameObject ClawObject;

        public static readonly string BodyPartType = "Hands";

        [NonSerialized]
        protected GameObjectBlueprint _Blueprint;

        public GameObjectBlueprint Blueprint
        {
            get
            {
                if (_Blueprint == null)
                {
                    _Blueprint = GameObjectFactory.Factory.GetBlueprint("BeastClaws");
                }

                return _Blueprint;
            }
        }
        
        public override string GetDescription()
        {
            return $"Your hands bear sharp sets of claws.";
        }
        public override bool CanLevel()
        {
            return false;
        }

        public override bool GeneratesEquipment()
        {
            return true;
        }


        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            Writer.WriteNamedFields(this, typeof(BeastClaws));
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            Reader.ReadNamedFields(this, typeof(BeastClaws));
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            GameObjectBlueprint blueprint = Blueprint;
            string partParameter = blueprint.GetPartParameter("MeleeWeapon", "Slot", "Hand");
            List<BodyPart> part = body.GetPart(partParameter);
            int level = base.Level;
            for (int i = 0; i < part.Count && i < 2; i++)
            {
                BodyPart bodyPart = part[i];
                if (bodyPart.DefaultBehavior == null || bodyPart.DefaultBehavior.GetBlueprint() != blueprint)
                {
                    bodyPart.DefaultBehavior = GameObject.Create(blueprint);
                    bodyPart.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "BeastClaws");
                }
            }
        }
    }
}