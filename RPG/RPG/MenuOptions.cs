using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class MenuOptions : Sc�neMenu
   {
      public BoutonR�glage BoutonMusique { get; private set; }
      public BoutonR�glage BoutonSons { get; private set; }
      public BoutonBool BoutonFullScreen { get; private set; }

      public MenuOptions(RPG jeu, string nomImage, GestionnaireDeSc�nes sc�nesMgr)
         : base(jeu, nomImage, sc�nesMgr)
      {
      }

      // Add() components
      public override void Initialize()
      {
         int volumeInitialMusique = 75;
         int volumeInitialSons = 75;

         BoutonMusique = new BoutonR�glage(Jeu, volumeInitialMusique, new Vector2(550, 400), "Volume Musique", "EagleSans48", Color.Black, Color.Red, SceneManager);
         BoutonSons = new BoutonR�glage(Jeu, volumeInitialSons, new Vector2(550, 500), "Volume Sons", "EagleSans48", Color.Black, Color.Red, SceneManager);
         BoutonFullScreen = new BoutonBool(Jeu, new Vector2(550, 600), 'x', "Full screen", "EagleSans48", Color.Black, Color.Red, SceneManager);

         ListeDes�l�ments.Add(new AfficheurFPS(Jeu, "Arial"));

         ListeDes�l�ments.Add(new BoutonTextuel(Jeu, new Vector2(200, 200), "Retour", "TrajanusRoman60", Color.Black, Color.Snow, Sc�nes.MenuPrincipal, SceneManager));
         ListeDes�l�ments.Add(BoutonMusique);
         ListeDes�l�ments.Add(BoutonSons);
         ListeDes�l�ments.Add(BoutonFullScreen); // 0.01 correct; 0.001 trop

         AjusterVolume(volumeInitialMusique, volumeInitialSons);

         base.Initialize();
      }

      protected override void LoadContent()
      {

         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         AjusterVolume();
         G�rerClavier();
         base.Update(gameTime);
      }

      // Appel�e lorsque la sc�ne est activ�e (Initialize, mais � chaque activation de la sc�ne)
      public override void Activate()
      {

      }

      protected override void G�rerClavier()
      {
         if (GestionInput.EstNouvelleTouche(Keys.Escape))
         {
            SceneManager.ChangerDeSc�ne(Sc�nes.MenuPrincipal);
         }
      }

      private void AjusterVolume()
      {
         Soundtrack.AdjustVolume("Music", BoutonMusique.Valeur);
         Soundtrack.AdjustVolume("Sounds", BoutonSons.Valeur);
      }

      private void AjusterVolume(int volMusique, int volSons)
      {
         Soundtrack.AdjustVolume("Music", (float)volMusique / 100f);
         Soundtrack.AdjustVolume("Sounds", (float)volSons / 100f);
      }

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }
   }
}