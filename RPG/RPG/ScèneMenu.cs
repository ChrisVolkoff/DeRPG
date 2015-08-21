using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   public class ScèneMenu : Scène
   {
      protected string NomImage { get; set; }
      protected Texture2D ImageMenu { get; set; }
      protected Rectangle RectAffichage { get; set; }

      public ScèneMenu(RPG jeu, string nomImage, GestionnaireDeScènes scènesMgr)
         : base(jeu, scènesMgr)
      {
         NomImage = nomImage;
      }

      public override void Initialize()
      {
         InitAffichage();
         base.Initialize();
      }

      protected override void LoadContent()
      {
         ImageMenu = GestionnaireDeTextures.Find(NomImage);
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         base.Update(gameTime);
      }

      // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
      public override void Activate() { }

      protected override void GérerClavier() { }

      public void InitAffichage()
      {
         RectAffichage = new Rectangle(0, 0, Jeu.Window.ClientBounds.Width, Jeu.Window.ClientBounds.Height);
      }

      public override void Draw(GameTime gameTime)
      {
         GestionSprites.Begin();
         GestionSprites.Draw(ImageMenu, RectAffichage, Color.White);
         GestionSprites.End();
         base.Draw(gameTime);
      }
   }
}