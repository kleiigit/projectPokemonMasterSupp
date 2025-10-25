using ProjetoPokemon.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoPokemon.Entities
{
    internal class EffectCard
    {
        public char TargetEffect { get; }
        public EffectType EffectType { get; }
        public int BonusEffect { get; }
        public string DescriptionEffect { get; }

        public EffectCard(char targetEffect, EffectType effectName, int bonusEffect)
        {
            TargetEffect = targetEffect;
            EffectType = effectName;
            BonusEffect = bonusEffect;
            DescriptionEffect = "";
        }

        public EffectCard(char targetEffect, EffectType effectType)
        {
            TargetEffect = targetEffect;
            EffectType = effectType;
            DescriptionEffect = "";
        }

        public static void CardEffectBattle(string card, BattleModifications trainer)
        {
            string[] cardsEffects = card.Split(';');
            foreach (string effect in cardsEffects)
            {
                string[] effectSplit = effect.Split('.');
                switch (effectSplit[0])
                {
                    case "roll":
                        trainer.CardBonus += int.Parse(effectSplit[1]);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
