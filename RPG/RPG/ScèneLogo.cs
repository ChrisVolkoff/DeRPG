using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AtelierXNA
{
    public class ScèneLogo : ScèneMenu
    {
        public ScèneLogo(RPG jeu, string nomImage, GestionnaireDeScènes scènesMgr)
            : base(jeu, nomImage, scènesMgr)
        {
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (GestionInput.EstNouvelleTouche(Keys.Space) || GestionInput.EstNouvelleTouche(Keys.Enter) || GestionInput.EstNouveauClicGauche())
            {
                SceneManager.ChangerDeScène(Scènes.MenuPrincipal);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }
    }
}
