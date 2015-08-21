using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class CharacterSelection : ScèneMenu
   {
      const int NB_CHOIX = 4;
      const float HAUTEUR_CHOIX = 200f;
      public CharacterSelection(RPG jeu, string nomImage, GestionnaireDeScènes scènesMgr)
         : base(jeu, nomImage, scènesMgr)
      {

      }

      // Add() components
      public override void Initialize()
      {
         ListeDesÉléments.Add(new AfficheurFPS(Jeu, "Arial"));
         
         for (int i = 1; i < NB_CHOIX+1; ++i)
         {
            ListeDesÉléments.Add(new BoutonTextuel(Jeu, new Vector2(Jeu.Window.ClientBounds.Width / 2, (float)((Jeu.Window.ClientBounds.Height*i+HAUTEUR_CHOIX) / (NB_CHOIX+1))), "Baldor Le Brave - Magicien", "FuturaLT60", Color.Azure, Color.CadetBlue, Scènes.Jeu, SceneManager));
         }
         Soundtrack.StartSongCue("m_menu");

         base.Initialize();
      }

      protected override void LoadContent()
      {
         
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         base.Update(gameTime);
      }



      // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
      public override void Activate()
      { }

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }

   }
}