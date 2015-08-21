using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{

   public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
   {
      const int POSITION_VIE_X = 160,
                POSITION_VIE_Y = 780,
                POSITION_RESSOURCE_X = 1273,
                POSITION_RESSOURCE_Y = 780;

      const int EXP_BAR_GAUCHE = 761,
               EXP_BAR_DROITE = 1100,
               EXP_BAR_BAS = 886,
               EXP_BAR_HAUT = 822;

      const int SPELLS_BOUTON_Y = 824,
                SPELLS_BOUTON_X1 = 432,
                SPELLS_BOUTON_X2 = 492,
                SPELLS_BOUTON_X3 = 552,
                SPELLS_BOUTON_X4 = 612,
                SPELLS_BOUTON_HAUTEUR = 58,
                SPELLS_BOUTON_LARGEUR = 56,
                STATS_X = 341,
                STATS_Y = 826,
                STATS_LARGEUR = 57,
                STATS_HAUTEUR = 57;


      RPG Jeu { get; set; }
      ScèneDeJeu ScèneJeu { get; set; }
      Héros Joueur {get;set;}
      Texture2D GUI { get; set; }
      Texture2D TextureVie { get; set; }
      Texture2D TextureRessource { get; set; }
      string NomGUI { get; set; }
      string NomTextureVie { get; set; }
      string NomTextureRessource { get; set; }
      BarreExp ExpBar { get; set; }
      public BoutonInfo[] Spells { get; private set; }
      InfoStats Stats { get; set; }

      public HUD(RPG jeu, ScèneDeJeu scenejeu, Héros joueur, string nomtexturegui, string nomtextureVie, string nomtextureRessource, string nomtextureExp,  float duréeAnimExp, string[] iconesSpells, string[] iconesSpellsAlt, string nomfontTextes, string nomtextBackground, string[] textfilenames, string[] alttextfilenames)
         : base(jeu)
      {
         Jeu = jeu;
         ScèneJeu = scenejeu;
         NomGUI = nomtexturegui;
         Joueur = joueur;
         NomTextureVie = nomtextureVie;
         NomTextureRessource = nomtextureRessource;
         Spells = new BoutonInfo[4]
         {
             new BoutonInfo(jeu, scenejeu, new Rectangle((int)(SPELLS_BOUTON_X1*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_Y*ScèneJeu.Scale.Y), (int)(SPELLS_BOUTON_LARGEUR*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_HAUTEUR*ScèneJeu.Scale.Y)), iconesSpells[0], iconesSpellsAlt[0], nomfontTextes, nomtextBackground, textfilenames[0], alttextfilenames[0]),
             new BoutonInfo(jeu, scenejeu, new Rectangle((int)(SPELLS_BOUTON_X2*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_Y*ScèneJeu.Scale.Y), (int)(SPELLS_BOUTON_LARGEUR*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_HAUTEUR*ScèneJeu.Scale.Y)), iconesSpells[1], iconesSpellsAlt[1], nomfontTextes, nomtextBackground, textfilenames[1], alttextfilenames[1]),
             new BoutonInfo(jeu, scenejeu, new Rectangle((int)(SPELLS_BOUTON_X3*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_Y*ScèneJeu.Scale.Y), (int)(SPELLS_BOUTON_LARGEUR*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_HAUTEUR*ScèneJeu.Scale.Y)), iconesSpells[2], iconesSpellsAlt[2], nomfontTextes, nomtextBackground, textfilenames[2], alttextfilenames[2]),
             new BoutonInfo(jeu, scenejeu, new Rectangle((int)(SPELLS_BOUTON_X4*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_Y*ScèneJeu.Scale.Y), (int)(SPELLS_BOUTON_LARGEUR*ScèneJeu.Scale.X), (int)(SPELLS_BOUTON_HAUTEUR*ScèneJeu.Scale.Y)), iconesSpells[3], iconesSpellsAlt[3], nomfontTextes, nomtextBackground, textfilenames[3], alttextfilenames[3]),
         };
         ExpBar = new BarreExp(jeu, scenejeu, nomtextureExp, new Rectangle(EXP_BAR_GAUCHE, EXP_BAR_HAUT, EXP_BAR_DROITE - EXP_BAR_GAUCHE, EXP_BAR_BAS - EXP_BAR_HAUT), duréeAnimExp);
         Stats = new InfoStats(jeu, scenejeu, joueur, new Rectangle((int)(STATS_X * ScèneJeu.Scale.X), (int)(STATS_Y * ScèneJeu.Scale.Y), (int)(STATS_LARGEUR * ScèneJeu.Scale.X), (int)(STATS_HAUTEUR * ScèneJeu.Scale.Y)), nomfontTextes, nomtextBackground);
      }

      public override void Initialize()
      {
         base.Initialize();
         ExpBar.Initialize();
         Stats.Initialize();
         for (int i = 0; i < Spells.Length; i++)
         {
             Spells[i].Initialize();
         }
      }

      protected override void LoadContent()
      {
         GUI = ScèneJeu.GestionnaireDeTextures.Find(NomGUI);
         TextureVie = ScèneJeu.GestionnaireDeTextures.Find(NomTextureVie);
         TextureRessource = ScèneJeu.GestionnaireDeTextures.Find(NomTextureRessource);
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
          
          base.Update(gameTime);
          ExpBar.Update(gameTime);
          
          for (int i = 0; i < Spells.Length; i++)
          {
              Spells[i].Update(gameTime);
          }
          Stats.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         int TailleGlobe = TextureVie.Width;
         Rectangle rectangleSourceVie = new Rectangle(0, (int)(TailleGlobe * (1 - Joueur.PtsVie / Joueur.PtsVieMax)), TailleGlobe, (int)(TailleGlobe * Joueur.PtsVie / Joueur.PtsVieMax));
         Rectangle rectangleSourceRessource = new Rectangle(0, (int)(TailleGlobe * (1 - Joueur.PtsRessource / Joueur.PtsRessourceMax)), TailleGlobe, (int)(TailleGlobe * Joueur.PtsRessource / Joueur.PtsRessourceMax));
         Jeu.GestionSprites.Begin();
         Jeu.GestionSprites.Draw(TextureVie, new Vector2(POSITION_VIE_X * ScèneJeu.Scale.X, (POSITION_VIE_Y + (TailleGlobe * (1 - Joueur.PtsVie / Joueur.PtsVieMax))) * ScèneJeu.Scale.Y), rectangleSourceVie, Color.White, 0, new Vector2(TailleGlobe / 2, TailleGlobe / 2), ScèneJeu.Scale, SpriteEffects.None, 0f);
         Jeu.GestionSprites.Draw(TextureRessource, new Vector2(POSITION_RESSOURCE_X * ScèneJeu.Scale.X, (POSITION_RESSOURCE_Y + (TailleGlobe * (1 - Joueur.PtsRessource / Joueur.PtsRessourceMax))) * ScèneJeu.Scale.Y), rectangleSourceRessource, Color.White, 0, new Vector2(TailleGlobe / 2, TailleGlobe / 2), ScèneJeu.Scale, SpriteEffects.None, 0f);
         Jeu.GestionSprites.Draw(GUI, new Rectangle(0, 0, Jeu.Window.ClientBounds.Width, Jeu.Window.ClientBounds.Height), Color.White);
         Jeu.GestionSprites.End();
         ExpBar.Draw(gameTime);
         for (int i = 0; i < Spells.Length; i++)
         {
             Spells[i].Draw(gameTime);
         }
         for (int i = 0; i < Spells.Length; i++)
         {
            Spells[i].DrawText(gameTime);
         }
         Stats.Draw(gameTime);
         Stats.DrawText(gameTime);
         base.Draw(gameTime);
         
      }
   }
}
