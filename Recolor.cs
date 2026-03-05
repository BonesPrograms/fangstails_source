using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World.Parts.Mutation;
using XRL.World;

namespace Fangs_Tails
{
    [HasWishCommand]
    public static class Wishes
    {
        [WishCommand(Command = "tail")]
        public static void Tail()
        {
            GameObject g = The.Player;
            if (Check(g, out var tail))
            {
                string tile = Popup.ShowColorPicker("Pick primary color:", includeNone: false);
                string detail = Popup.ShowColorPicker("Pick secondary color:", includeNone: false);
                tail.SetColor(tile, detail);
            }
            else
            {
                Popup.Show("You don't have a tail!");
            }

        }
        public static bool Check(GameObject g, out BeastTail tail)
        {
            tail = g.GetPart<BeastTail>();
            return tail != null;
        }
    }
}