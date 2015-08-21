using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class MenuOptions : ScèneMenu
   {
      public BoutonRéglage BoutonMusique { get; private set; }
      public BoutonRéglage BoutonSons { get; private set; }
      public BoutonBool BoutonFullScreen { get; private set; }

      public MenuOptions(RPG jeu, string nomImage, GestionnaireDeScènes scènesMgr)
         : base(jeu, nomImage, scènesMgr)
      {
      }

      // Add() components
      public override void Initialize()
      {
         int volumeInitialMusique = 75;
         int volumeInitialSons = 75;

         BoutonMusique = new BoutonRéglage(Jeu, volumeInitialMusique, new Vector2(550, 400), "Volume Musique", "EagleSans48", Color.Black, Color.Red, SceneManager);
         BoutonSons = new BoutonRéglage(Jeu, volumeInitialSons, new Vector2(550, 500), "Volume Sons", "EagleSans48", Color.Black, Color.Red, SceneManager);
         BoutonFullScreen = new BoutonBool(Jeu, new Vector2(550, 600), 'x', "Full screen", "EagleSans48", Color.Black, Color.Red, SceneManager);

         ListeDesÉléments.Add(new AfficheurFPS(Jeu, "Arial"));

         ListeDesÉléments.Add(new BoutonTextuel(Jeu, new Vector2(200, 200), "Retour", "TrajanusRoman60", Color.Black, Color.Snow, Scènes.MenuPrincipal, SceneManager));
         ListeDesÉléments.Add(BoutonMusique);
         ListeDesÉléments.Add(BoutonSons);
         ListeDesÉléments.Add(BoutonFullScreen); // 0.01 correct; 0.001 trop

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
         GérerClavier();
         base.Update(gameTime);
      }

      // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
      public override void Activate()
      {

      }

      protected override void GérerClavier()
      {
         if (GestionInput.EstNouvelleTouche(Keys.Escape))
         {
            SceneManager.ChangerDeScène(Scènes.MenuPrincipal);
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