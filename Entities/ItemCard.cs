

using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities
{
    enum RarityCard
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Unique,
    }
    internal class ItemCard
    {
        public int Id { get; }
        public string Name { get;}
        public RarityCard Rarity { get; } = RarityCard.Common;
        public TypeItemCard Type { get;}
        public List<EffectManager> Effects = new List<EffectManager>();
        public string Description { get;}

        public ItemCard(int id, string name, RarityCard rarity, TypeItemCard type, List<EffectManager> effects, string description)
        {
            Id = id;
            Name = name;
            Rarity = rarity;
            Type = type;
            Effects = effects;
            Description = description;
        }

        public void BattleCard(BattlerManager user, BattlerManager target)
        {
            if (user.UsedCard == null) return;
            if (Type == TypeItemCard.Battle)
            {
                foreach (EffectManager effect in Effects)
                {
                    BattlerManager targetEffects = effect.TargetEffect == 'W'? user : target;
                    string[] condSplit = new string[1];
                    if(effect.EffectCond != null) condSplit = effect.EffectCond.Split(','); // number of target, effec >>>
                    switch (effect.EffectType)
                    {
                        case EffectType.ADDROLL:
                            targetEffects.CardBonus += effect.BonusEffect;
                            BattleLog.AddLog($"{user.UsedCard.Name} add bonus roll of {targetEffects.SelectedPokemon.NickPokemon}.");
                            break;
                        case EffectType.TWODICES:
                            targetEffects.NumberDices += effect.TargetEffect == 'W' ? +1 : -1;
                            BattleLog.AddLog($"{user.UsedCard.Name} change {targetEffects.SelectedPokemon.NickPokemon} dices.");
                            break;
                        case EffectType.THREEDICES:
                            targetEffects.NumberDices += effect.TargetEffect == 'W' ? 2 : -2;
                            BattleLog.AddLog($"{user.UsedCard.Name} change {targetEffects.SelectedPokemon.NickPokemon} dices.");
                            break;
                        case EffectType.HEAL:
                            for (int i = 0; i < int.Parse(condSplit[0]); i++)
                            {
                                List<ProfilePokemon> listPokemon = new List<ProfilePokemon>();
                                if (user.TrainerBox != null)
                                {
                                    if(effect.EffectCond == "KO,STATUS")
                                    {
                                        listPokemon = user.TrainerBox.ListPokemon.Where(p => p.Conditions != StatusConditions.NORMAL).ToList();
                                    }
                                    else if (condSplit[1] == "KO")
                                    {
                                        listPokemon = user.TrainerBox.ListPokemon.Where(p => p.Conditions == StatusConditions.KNOCKED).ToList();
                                    }
                                    else if (condSplit[1] == "STATUS")
                                    {
                                        listPokemon = user.TrainerBox.ListPokemon.Where(p => p.Conditions != StatusConditions.NORMAL 
                                        && p.Conditions != StatusConditions.KNOCKED).ToList();
                                    }  
                                }

                                if (listPokemon.Count != 0)
                                {
                                    int index = ConsoleMenu.ShowMenu(ConsoleColor.Magenta, listPokemon.Select(m => m.ToString()).ToList(), "Select a Pokemon to healing");
                                    listPokemon[index].Conditions = StatusConditions.NORMAL;
                                    BattleLog.AddLog($"{listPokemon[index].NickPokemon} was healed.");
                                }
                                
                            }
                            break;
                        case EffectType.DICESIDE:
                            targetEffects.UsedMove.ChangeSide(effect.BonusEffect);
                            BattleLog.AddLog($"{user.UsedCard.Name} change {targetEffects.SelectedPokemon.NickPokemon} dices sides.");
                            break;
                    }
                    BattleLog.AddLog($"({user.UsedCard.Description})");
                }
            }
        }
        public string AttachCard()
        {
            string effectCard = "";
            if (Type == TypeItemCard.Attach)
            {
                foreach (EffectManager effect in Effects)
                {
                    string[] effectCond;
                    if (effect.EffectCond != null) effectCond = effect.EffectCond.Split(',');
                    switch (effect.EffectType)
                    {
                        // weather
                        case EffectType.SNOW:
                            effectCard += "snowcard;";
                            break;
                        case EffectType.RAIN:
                            effectCard += "raincard;";
                            break;
                        case EffectType.SAND:
                            effectCard += "sandcard;";
                            break;
                        case EffectType.SUNNYDAY:
                            effectCard += "suncard;";
                            break;
                        // trigger
                        case EffectType.ROLL:
                            effectCard += "rollTime;";
                            break;
                        case EffectType.STATUS:
                            effectCard += "statusTime;";
                            break;
                        // mod
                        case EffectType.ADDPOWER:
                            effectCard += $"addPower.{effect.BonusEffect};";
                            break;
                        case EffectType.IMMUNE:
                            if(effect.EffectCond != null)
                            {
                                if(effect.EffectCond == EffectType.STATUS.ToString())
                                {

                                }
                                else
                                {
                                    if (!Enum.TryParse(effect.EffectCond, out EffectType typeImmune))
                                    {
                                        effectCard += $"imune.{typeImmune}";
                                    }  
                                }
                            }
                            break;
                        
                            
                    }
                }   
            }
            if (effectCard == "")
                return "";
                else
                return effectCard;
        }
        public int CatchCard(Pokemon catchPokemon)
        {
            int bonusCatch = 0;
            if (Type == TypeItemCard.Catch)
            {
                foreach (EffectManager effect in Effects)
                {
                    bonusCatch += effect.BonusEffect;
                    string[] effectCond;
                    if (effect.EffectCond != null)
                    {
                        effectCond = effect.EffectCond.Split(',');
                        switch (effect.EffectType)
                        {
                            case EffectType.TYPE:
                                bonusCatch += effectCond[0] == catchPokemon.Type.ToString().ToUpper() ? int.Parse(effectCond[1]) : 1;
                                break;
                            case EffectType.COLOR:
                                bonusCatch += effectCond[0] == catchPokemon.Color.ToString().ToUpper() ? int.Parse(effectCond[1]) : 1;
                                break;
                        }
                    }
                    else
                    {
                        if (effect.EffectType == EffectType.ESPECIAL)
                        {
                            if (Id == 35) // MASTER BALL
                            {
                                bonusCatch += 99;
                            }
                        }


                    }
                }
                
            }
            return bonusCatch;
        }

        public ItemCard Copy()
        {
            ItemCard copiedItem = new ItemCard(
                Id,
                Name,
                Rarity,
                Type,
                Effects,
                Description
            );
            return copiedItem;
        }
        public override string ToString()
        {
            return Name + $" - {Rarity.ToString()} {Type.ToString()} Card ({Description})";
        }
    }
}
