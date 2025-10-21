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
        public char TargetEffect { get; set; }
        public EffectType EffectType { get; set; }
        public int BonusEffect { get; set; }
        public string DescriptionEffect { get; set; }

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
    }
}
