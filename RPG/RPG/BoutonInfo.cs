using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace AtelierXNA
{
    public class BoutonInfo : Microsoft.Xna.Framework.DrawableGameComponent
    {
       const int SPACE_BETWEEN_BORDER_AND_TEXT = 5;

        float TempsDepuisMAJ { get; set; }

        RPG Jeu { get; set; }
        ScèneDeJeu ScèneJeu { get; set; }
        Rectangle Emplacement { get; set; }
        Texture2D Texture { get; set; }
        string NomTexture { get; set; }
        Texture2D ToggleTexture { get; set; }
        string NomToggleTexture { get; set; }
        SpriteFont Font { get; set; }
        string NomFont { get; set; }
        Texture2D TextBackground { get; set; }
        string NomTextBackground { get; set; }
        protected string Text { get; set; }
        string TextFileName { get; set; }
        string AltText { get; set; }
        string AltTextFileName { get; set; }
        bool DisplayText { get; set; }
        public bool Toggled { get; set; }

        /// <param name="texture">Peut être nul; alors rien n'apparaitra et 
        /// le bouton ne sera qu'une zone pointable avec du texte</param>
        public BoutonInfo(RPG jeu, ScèneDeJeu scenejeu, Rectangle emplacement, string nomtexture, string nomtoggleTexture, string nomfont, string nomtextBackground, string textfilename, string alttextfilename)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scenejeu;
            Emplacement = emplacement;
            NomTexture = nomtexture;
            NomToggleTexture = nomtoggleTexture;
            NomFont = nomfont;
            NomTextBackground = nomtextBackground;
            TextFileName = textfilename;
            AltTextFileName = alttextfilename;
            
        }

        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
            Toggled = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
           Font = ScèneJeu.GestionnaireDeFonts.Find(NomFont);

           TextBackground = ScèneJeu.GestionnaireDeTextures.Find(NomTextBackground);
           if (NomTexture != null)
           {
              Texture = ScèneJeu.GestionnaireDeTextures.Find(NomTexture);
           }
           if (NomToggleTexture != null)
           {
              ToggleTexture = ScèneJeu.GestionnaireDeTextures.Find(NomToggleTexture);
           }
           Text = null;
           if (TextFileName != null)
           {
              Text = new StreamReader(TextFileName).ReadToEnd();
           }
           AltText = null;
           if (AltTextFileName != null)
           {
              AltText = new StreamReader(AltTextFileName).ReadToEnd();
           }
           base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            TempsDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
               DisplayText = (Emplacement.Contains(ScèneJeu.GestionInput3D.ÉtatSouris.X, ScèneJeu.GestionInput3D.ÉtatSouris.Y));
                
            }
            if (DisplayText)
            {
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScèneJeu.GestionSprites.Begin();
            if (Toggled && ToggleTexture != null) 
            {
                ScèneJeu.GestionSprites.Draw(ToggleTexture, Emplacement, Color.White);
            }
            else
            {
               if (Texture != null)
               {
                  ScèneJeu.GestionSprites.Draw(Texture, Emplacement, Color.White);
               }
            }
            
            ScèneJeu.GestionSprites.End();
            base.Draw(gameTime);
        }

        public void DrawText(GameTime gameTime)
        {
           if (this is InfoStats)
           {
           }
           if (DisplayText)
           {

              Vector2 sourisPos = ScèneJeu.GestionInput3D.GetPositionSouris();
              //Vector2 positionText = new Vector2(0, 0);
              ScèneJeu.GestionSprites.Begin();
              if (Toggled && AltText != null)
              {
                 Vector2 tailleTexte = Font.MeasureString(AltText);
                 Vector2 positionText = new Vector2(sourisPos.X, sourisPos.Y - tailleTexte.Y);
                 ScèneJeu.GestionSprites.Draw(TextBackground, new Rectangle((int)positionText.X - SPACE_BETWEEN_BORDER_AND_TEXT, (int)positionText.Y - SPACE_BETWEEN_BORDER_AND_TEXT, (int)tailleTexte.X + SPACE_BETWEEN_BORDER_AND_TEXT * 2, (int)tailleTexte.Y + SPACE_BETWEEN_BORDER_AND_TEXT * 2), Color.White);
                 ScèneJeu.GestionSprites.DrawString(Font, AltText, positionText, Color.Beige);
              }
              else
              {
                 if (Text != null)
                 {
                    Vector2 tailleTexte = Font.MeasureString(Text);
                    Vector2 positionText = new Vector2(sourisPos.X, sourisPos.Y - tailleTexte.Y);
                    ScèneJeu.GestionSprites.Draw(TextBackground, new Rectangle((int)positionText.X - SPACE_BETWEEN_BORDER_AND_TEXT, (int)positionText.Y - SPACE_BETWEEN_BORDER_AND_TEXT, (int)tailleTexte.X + SPACE_BETWEEN_BORDER_AND_TEXT * 2, (int)tailleTexte.Y + SPACE_BETWEEN_BORDER_AND_TEXT * 2), Color.White);
                    ScèneJeu.GestionSprites.DrawString(Font, Text, positionText, Color.Beige);
                 }
              }
              ScèneJeu.GestionSprites.End();
           }
        }
    }
}
