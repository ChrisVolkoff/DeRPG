using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class CharacterSelection : Sc�neMenu
   {
      const int NB_CHOIX = 4;
      const float HAUTEUR_CHOIX = 200f;
      public CharacterSelection(RPG jeu, string nomImage, GestionnaireDeSc�nes sc�nesMgr)
         : base(jeu, nomImage, sc�nesMgr)
      {

      }

      // Add() components
      public override void Initialize()
      {
         ListeDes�l�ments.Add(new AfficheurFPS(Jeu, "Arial"));
         
         for (int i = 1; i < NB_CHOIX+1; ++i)
         {
            ListeDes�l�ments.Add(new BoutonTextuel(Jeu, new Vector2(Jeu.Window.ClientBounds.Width / 2, (float)((Jeu.Window.ClientBounds.Height*i+HAUTEUR_CHOIX) / (NB_CHOIX+1))), "Baldor Le Brave - Magicien", "FuturaLT60", Color.Azure, Color.CadetBlue, Sc�nes.Jeu, SceneManager));
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



      // Appel�e lorsque la sc�ne est activ�e (Initialize, mais � chaque activation de la sc�ne)
      public override void Activate()
      { }

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }

   }
}