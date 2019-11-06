using Lunar.Core;
using Lunar.Core.World.Structure.Attribute;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Conversation;

namespace Lunar.Server.World.Structure.Attribute
{
    internal class DialogueTileAttributeActionHandler : TileAttributeActionHandler
    {
        public override void OnInitalize(ITileAttributeArgs args)
        {
        }

        public override void OnPlayerEntered(ITileAttributeArgs args)
        {
            Player player = (args as TileAttributePlayerArgs).Player;

            string dialogueName = (args.Attribute as StartDialogueTileAttribute).DialogueName;
            string branchName = (args.Attribute as StartDialogueTileAttribute).BranchName;

            Engine.Services.Get<DialogueManager>().Get(dialogueName).Start(branchName, player);
        }

        public override void OnPlayerLeft(ITileAttributeArgs args)
        {
        }

        public override void OnUpdate(ITileAttributeArgs args)
        {
        }
    }
}