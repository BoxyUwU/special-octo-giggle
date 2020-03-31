using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework.Input;

namespace GigglyLib.Systems
{
    public class ReplayInputSys : ISystem<float>
    {
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose() { throw new NotImplementedException(); }

        public void Update(float state)
        {
            if (Game1.ReplayCounter == Game1.ReplayData.Count-1 && (byte)Game1.ReplayIntraByteCounter == Game1.ReplayData[4])
                return;

            var keyState = Keyboard.GetState();
            if (!keyState.IsKeyDown(Keys.Space))
                return;

            byte data = Game1.ReplayData[Game1.ReplayCounter];
            data = (byte)(data >> (Game1.ReplayIntraByteCounter * 2));
            data = (byte)(data & 3);

            CMoveAction action = new CMoveAction();
            switch (data)
            {
                case 0:
                    action.DistX = -1;
                    break;
                case 1:
                    action.DistX = 1;
                    break;
                case 2:
                    action.DistY = -1;
                    break;
                case 3:
                    action.DistY = 1;
                    break;
                default:
                    throw new Exception("Invalid move data read in");
            }
            Game1.Player.Set(action);

            Game1.currentRoundState++;
            Config.SFX["player-move"].Play();

            Game1.ReplayIntraByteCounter++;
            if (Game1.ReplayIntraByteCounter == 4)
            {
                Game1.ReplayIntraByteCounter = 0;
                Game1.ReplayCounter++;
            }
        }
    }
}
