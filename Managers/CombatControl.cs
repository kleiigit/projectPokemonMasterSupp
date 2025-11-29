using DocumentFormat.OpenXml.Drawing.Charts;
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Managers
{
    static class CombatControl
    {
        private static readonly ConsoleColor atkColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor defColor = ConsoleColor.Magenta;
        public static bool ConditionCheck(Battler battler)
        {
            if (battler.Pokemon.Conditions == StatusConditions.PARALYZED)
            {
                if (!BattleConditions.ParalyzedRoll())
                {
                    BattleLog.AddLog(battler + " is paralyzed and cannot attack!");
                    return false;
                }
            }
            else if (battler.Pokemon.Conditions == StatusConditions.SLEEP)
            {
                battler.Pokemon.ConditionCount--;
                if (battler.Pokemon.ConditionCount == 0)
                {
                    BattleLog.AddLog(battler + " woke up!");
                    battler.Pokemon.Conditions = StatusConditions.NORMAL; return true;
                }
                else
                {
                    // SleepTalk
                    EffectControl.SleepTalk(battler);
                    BattleLog.AddLog(battler + " is still sleeping...");
                    return false;
                }
            }
            else if (battler.Pokemon.Conditions == StatusConditions.FROZEN)
            {
                if (BattleConditions.FrozenRoll())
                {
                    battler.Pokemon.Conditions = StatusConditions.NORMAL;
                    BattleLog.AddLog(battler + " has been unfrozen and can attack!");
                    return true;
                }
                else BattleLog.AddLog(battler + " is still frozen."); return false;
            }
            return true;
        }
        public static int ConditionMod(Battler battler)
        {
            ProfilePokemon pokemon = battler.Pokemon;
            if (pokemon.Conditions != StatusConditions.NORMAL)
            {
                StatusConditions status = pokemon.Conditions;
                string messageSts = pokemon.Name + " is ";

                switch (status)
                {
                    case StatusConditions.PARALYZED:
                        BattleLog.AddLog(messageSts + "paralyzed!");
                        break;
                    case StatusConditions.FROZEN:
                        BattleLog.AddLog(messageSts + "frozen!");

                        break;
                    case StatusConditions.BURNED:
                        BattleLog.AddLog(messageSts + "burn!");
                        return battler.GetUsedMovePower() > 0 ? -1 : 0;

                    case StatusConditions.SLEEP:
                        BattleLog.AddLog(messageSts + "asleep!");
                        if (pokemon.ConditionCount > 0)
                        {
                            BattleLog.AddLog(pokemon.Name + " asleep and cannot attack!");
                            return 0;
                        }
                        break;
                    case StatusConditions.CONFUSED:
                        BattleLog.AddLog(messageSts + "confused!");
                        if (battler.Total.Roll % 2 != 0)
                        {
                            BattleLog.AddLog(pokemon.Name + " is confused and hurt itself! Roll turns 0!");
                            battler.Total.Roll = 0;
                        }
                        break;
                }

            }
            return 0;
        }
        public static int RollDices(Battler battler)
        {
            int bonusDice = battler.Total.NumberDices;
            int diceCount = Math.Max(1, 1 + Math.Abs(bonusDice)); // 1 padrão + adicionais

            battler.Total.Rolls = new int[diceCount];
            string sumDices = "";

            for (int i = 0; i < diceCount; i++)
            {
                int roll = DiceRollService.RollDice(1, battler.GetUsedMoveDiceSide());

                // Corrige valores fora do limite (7 → 5, 8 → 6)
                if (roll == 7)
                {
                    BattleLog.AddLog($"ROLL {roll} CRÍTICAL, turn 5");
                    roll = 5;
                }
                else if (roll == 8)
                {
                    BattleLog.AddLog($"ROLL {roll} CRÍTICAL, turn 6");
                    roll = 6;
                }

                battler.Total.Rolls[i] = roll;
                sumDices += $" {roll} +";
                BattleLog.AddLog($"ROLL:{roll} - {battler} move {battler.GetUsedMoveName()}.");
            }

            int result;

            // Se o movimento possui SOMADICES, a soma prevalece, sem interferência do NumberDices
            if (battler.CheckEffectUsedMove(EffectType.SOMADICES))
            {
                if (battler.Total.Rolls.Length > 1) BattleLog.AddLog("ROLL RESULT: " + sumDices[..^1] + $" = {battler.Total.Rolls.Sum()}.\n");
                result = battler.Total.Rolls.Sum();
            }
            else
            {
                // Caso contrário, vantagem/desvantagem afeta o melhor/pior resultado
                if (bonusDice < 0)
                    result = battler.Total.Rolls.Min(); // Desvantagem
                else if (bonusDice > 0)
                    result = battler.Total.Rolls.Max(); // Vantagem
                else
                    result = battler.Total.Rolls[0];    // Normal
            }

            // Efeito de precisão
            if (battler.CheckEffectUsedMove(EffectType.PRECISION) && result < 3 && battler.GetUsedMove().DiceSides > 5)
            {
                BattleLog.AddLog($"** Effect Precision of {battler.GetUsedMoveName()}. Roll result {result} modified to 3.");
                result = 3;
            }

            return result;
        }
        public static void MoveEffect(Battler user, Battler targett) // aplica os efeito dos movimentos
        {
            if (user.GetEffectUsedMove == null)
                return;
            if (user.CheckUsedMoveTargetEffect('B') && targett.HasBuffEffect(EffectType.PROTECT)) return;

            user.Total.RedRoll = DiceRollService.RollD6();

            if (!user.CheckEffectRoll(user.Total.RedRoll)) return;

            foreach (var effect in user.GetUsedMove().Effects)
            {
                Battler targetEffects = effect.TargetEffect == 'B' ? targett : user;

                switch (effect.EffectType)
                {
                    case EffectType.TWODICES:
                        if (targett.GetUsedMoveID() == 168 && effect.TargetEffect == 'B')
                        {
                            BattleLog.AddLog($"{targett.Pokemon.Name} avoids the move’s effects with {targett.GetUsedMoveName()}");
                            return;
                        }
                        MoreDicesEffect(effect, user, targett, 1);
                        break;
                    case EffectType.SOMADICES:
                        if (targett.GetUsedMoveID() == 168 && effect.TargetEffect == 'B')
                        {
                            BattleLog.AddLog($"{targett.Pokemon.Name} avoids the move’s effects with {targett.GetUsedMoveName()}");
                            return;
                        }
                        user.Total.NumberDices++;
                        break;
                    case EffectType.THREEDICES:
                        if (targett.GetUsedMoveID() == 168 && effect.TargetEffect == 'B')
                        {
                            BattleLog.AddLog($"{targett.Pokemon.Name} avoids the move’s effects with {targett.GetUsedMoveName()}");
                            return;
                        }
                        MoreDicesEffect(effect, user, targett, 2);
                        break;

                    case EffectType.BURN:
                        StatusConditionsTrigger(targetEffects, StatusConditions.BURNED);
                        break;
                    case EffectType.CONFUSION:
                        StatusConditionsTrigger(targetEffects, StatusConditions.CONFUSED);
                        break;
                    case EffectType.PARALYZE:
                        StatusConditionsTrigger(targetEffects, StatusConditions.PARALYZED);
                        break;
                    case EffectType.SLEEP:
                        StatusConditionsTrigger(targetEffects, StatusConditions.SLEEP);
                        break;
                    case EffectType.FREEZE:
                        StatusConditionsTrigger(targetEffects, StatusConditions.FROZEN);
                        break;
                    case EffectType.POISON:
                        StatusConditionsTrigger(targetEffects, StatusConditions.POISONED);
                        break;


                    case EffectType.KO:
                        targetEffects.Pokemon.Conditions = StatusConditions.KNOCKED;
                        BattleLog.AddLog($"** {targetEffects.Pokemon.Name} will be knocked out at the end of the round!");
                        break;

                    case EffectType.RECHARGE:
                        user.RechargeMove();
                        BattleLog.AddLog($"** {user.GetUsedMoveName()} can only be used once per battle.");
                        break;
                    case EffectType.STATUS: //venoshock principalmente
                        if (targetEffects.Pokemon.Conditions != StatusConditions.NORMAL) user.Total.RollBonus += 2;
                        break;

                    case EffectType.RAIN: // RAIN DANCE
                        FieldControlService.ChangeWeather(FieldCards.RAIN);
                        BattleLog.AddLog(user + $" used {user.GetUsedMoveName()}, and it started to rain.\n");
                        break;
                    case EffectType.SUNNYDAY: // SUNNY DAY
                        FieldControlService.ChangeWeather(FieldCards.SUNNYDAY);
                        BattleLog.AddLog(user + $" used {user.GetUsedMoveName}, and the sun began to shine.\n");
                        break;
                    case EffectType.SNOW: // 
                        FieldControlService.ChangeWeather(FieldCards.SNOW);
                        BattleLog.AddLog(user + $" used {user.GetUsedMoveName}, and it started to snow.\n");
                        break;
                    case EffectType.SAND: // SANDSTORM
                        FieldControlService.ChangeWeather(FieldCards.SANDSTORM);
                        BattleLog.AddLog(user + $" used {user.GetUsedMoveName}, and the storm covered the field.\n");
                        break;

                    case EffectType.CHANGE: // trade pokemon
                        
                        if (user is BattlerTest)
                        {
                            break; 
                        }
                        if (effect.TargetEffect == 'B')
                        {
                            targett.Pokemon = targett.ChangeRandomPokemon();

                            if (ConditionCheck(targett))
                            {
                                targett.SelectMove(user); // precisa melhorar
                            }
                        }
                        else
                        {
                            if (user is BattlerPlayer)
                            {
                                BattlerPlayer player = (BattlerPlayer)user;
                                user.Pokemon = player.TrainerBox.SelectPokemon(atkColor);
                            }
                        }
                        break;
                    case EffectType.CARD:
                        if (user is BattlerPlayer)
                        {
                            BattlerPlayer player = (BattlerPlayer)user;
                            player.TrainerBox.DrawItemCard(1);
                            BattleLog.AddLog($"{player.TrainerName} draw a item card");
                        }
                        break;
                    case EffectType.NERF:
                        user.BuffsAndDebuffs.Add(EffectType.NERF);
                        BattleLog.AddLog(user.Pokemon + " weakened itself by using the move " + user.GetUsedMovePower());
                        break;
                    case EffectType.ESPECIAL:
                        EspecicalMoves(user, targett);
                        break;
                }
            }
        }
        public static void MoreDicesEffect(EffectManager effect, Battler user, Battler targets, int n) // melhorar para receber threedices
        {
            bool targetIsEnemy = effect.TargetEffect == 'B';

            if (targetIsEnemy)
            {
                targets.Total.NumberDices -= n;
            }
            else // 'W'
            {
                user.Total.NumberDices += n;
            }
        }
        public static void ApplyHalfLevelEffect(Battler user, Battler targets)
        {
            var move = user.MovesPokemon.FirstOrDefault(m => m.Effects.Any(e => e.EffectType == EffectType.HALFLEVEL));
            if (move != null)
            {
                var effect = move.Effects.FirstOrDefault(e => e.EffectType == EffectType.HALFLEVEL);

                if (effect != null)
                {
                    ProfilePokemon target = effect.TargetEffect == 'B' ? targets.Pokemon : user.Pokemon;
                    int baseLevel = target.LevelPokemon();
                    move.HalfLevelPower(ref baseLevel);
                }
            }
        }
        public static void StatusConditionsTrigger(Battler battleTarget, StatusConditions status) // quando ganha o status
        {
            ProfilePokemon target = battleTarget.Pokemon;
            BattleLog.AddLog("teste st");
            // Aplica a condição apenas se o Pokémon estiver em NORMAL
            if (battleTarget.BuffsAndDebuffs.Any(p => p == EffectType.SAFEGUARD)) return;
            if (target.Conditions == StatusConditions.NORMAL)
            {
                target.Conditions = status;
                if (status == StatusConditions.SLEEP)
                {
                    BattleLog.AddLog(target.Name + " was put to sleep!");
                    target.ConditionCount = BattleConditions.SleepRoll();
                    target.CanAttack = false;

                    target.ConditionCount--;
                    if (target.ConditionCount == 0)
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        BattleLog.AddLog(target.Name + " woke up!");
                        target.CanAttack = true;
                    }
                    else battleTarget.SetUsedMove(Move.Null());
                }
                else if (status == StatusConditions.PARALYZED)
                {
                    BattleLog.AddLog(target.Name + " was paralyzed!");
                    if (!BattleConditions.ParalyzedRoll())
                    {
                        BattleLog.AddLog(target.Name + " is paralyzed and cannot attack!");
                        target.CanAttack = false;
                        battleTarget.SetUsedMove(Move.Null());
                    }
                }
                else if (status == StatusConditions.FROZEN)
                {
                    BattleLog.AddLog(target.Name + " was frozen!");
                    if (!BattleConditions.FrozenRoll())
                    {
                        BattleLog.AddLog(target.Name + " is still frozen and cannot attack!");
                        target.CanAttack = false;
                        battleTarget.SetUsedMove(Move.Null());
                    }
                    else
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        BattleLog.AddLog(target.Name + " has been unfrozen and can attack!");
                        target.CanAttack = true;
                    }
                }
                else if (status == StatusConditions.POISONED)
                {
                    BattleLog.AddLog(target.Name + " was poisoned!");
                }
            }
        }
        public static void EspecicalMoves(Battler user, Battler opponent)
        {
            Move move = user.GetUsedMove();
            switch (move.MoveID)
            {
                case 29: // Defog
                    FieldControlService.ChangeWeather(FieldCards.NORMAL);
                    FieldControlService.ChangeField(FieldCards.NORMAL);
                    FieldControlService.ChangeTrap(FieldCards.NORMAL);
                    BattleLog.AddLog($"{move.Name} cleared the battlefield!");
                    break;
                case 73: // Disable
                    List<Move> moveList = opponent.MovesPokemon.Where(p => p.CanUse).ToList();
                    if (moveList.Count > 1)
                    {
                        int index = 0;
                        if (user is BattlerTest)
                        {
                            index = DiceRollService.RollDice(1, opponent.MovesPokemon.Count) - 1;
                        }
                        else
                        {
                            index = ConsoleMenu.ShowMenu(ConsoleColor.Cyan, opponent.MovesPokemon.Select(m => m.ToString()).ToList(),
                                "Select a move to disable. The move can’t be used until the end of the battle.");
                        }
                        opponent.MovesPokemon[index].RechargeMove();
                        BattleLog.AddLog(opponent.MovesPokemon[index].Name + " it's disabled!");
                    }
                    else BattleLog.AddLog("Disable didn't work correctly!");
                    break;

                case 54: // Metronome
                    Random random = new Random();
                    int n;
                    n = random.Next(0, DataLists.AllMoves.Count);// } while (n == 54);
                    Move randomMove = DataLists.AllMoves[n];
                    var copiedEffects = randomMove.Effects.Select(e =>
                    new EffectManager(e.TargetEffect, e.EffectType, e.BonusEffect, e.EffectCond, randomMove.MoveID)).ToList();

                    Move newMove = new Move(randomMove.MoveID, randomMove.Type, randomMove.Name, randomMove.Power,
                        randomMove.Effects, randomMove.DiceSides, randomMove.EffectRoll);
                    if (newMove.Type == user.Pokemon.Pokemon.Type || newMove.Type == user.Pokemon.Pokemon.StabType) newMove.StabMove();
                    user.SetUsedMove(newMove);
                    BattleLog.AddLog($"Metronome turned into the move {randomMove.Name}. \n{randomMove}");
                    MoveEffect(user, opponent);
                    break;

                case 88: //smack down
                    opponent.BuffsAndDebuffs.Add(EffectType.DOWNED);
                    BattleLog.AddLog($"{user.Pokemon.Name} knocked out {opponent.Pokemon.Name} with {user.GetUsedMoveName()}!");
                    break;
                case 98: // tri attack
                    int t = DiceRollService.RollD6();
                    StatusConditions[] effects =
                        { StatusConditions.PARALYZED, StatusConditions.PARALYZED,
                          StatusConditions.BURNED, StatusConditions.BURNED,
                          StatusConditions.FROZEN, StatusConditions.FROZEN };

                    StatusConditionsTrigger(opponent, effects[t - 1]);
                    break;
                case 36: // mirror move
                    BattleLog.AddLog($"** The move {opponent.GetUsedMoveName} has been copied!");
                    if (opponent.GetUsedMoveID() != 36)
                    {
                    user.SetUsedMove(opponent.GetUsedMove());
                    MoveEffect(user, opponent);
                    }
                    break;
                case 124: // counter
                    BattleLog.AddLog($"** COUNTER! The power of move has been changed to " + opponent.GetUsedMovePower());
                    user.SetPower(opponent.GetUsedMovePower());
                    break;
                case 176: // torment
                    opponent.BuffsAndDebuffs.Add(EffectType.ENRAGED);
                    BattleLog.AddLog(opponent.Pokemon.Name + "  is enraged!");
                    break;
                case 177: // taunt
                    opponent.BuffsAndDebuffs.Add(EffectType.TAUNT);
                    BattleLog.AddLog(opponent.Pokemon.Name + "  is taunted!");
                    if (opponent.GetUsedMove().Power == 0 && opponent.Pokemon.CanAttack == true)
                    {
                        opponent.SelectMove(user);
                    }

                    break;
                case 179: // protect
                    opponent.SetPower(0);
                    user.BuffsAndDebuffs.Add(EffectType.PROTECT);
                    BattleLog.AddLog(user.Pokemon.Name + "  is protected!");
                    break;
                case 180: // roost
                    if (user.Pokemon.Pokemon.Type == TypePokemon.Flying)
                    {
                        user.Pokemon.Pokemon.ChangeType(TypePokemon.Normal);
                        BattleLog.AddLog($"{user.Pokemon.GetName()} lost its Flying-Type");
                    }
                    break;
                case 165: // safe guard
                    user.BuffsAndDebuffs.Add(EffectType.SAFEGUARD); // ***** melhorar
                    BattleLog.AddLog(user.Pokemon.GetName() + " cannot be affected by status conditions!");
                    if (user.Pokemon.Conditions != StatusConditions.KNOCKED) user.Pokemon.Conditions = StatusConditions.NORMAL;
                    break;
                case 187: // facade
                    if (opponent.Pokemon.Conditions != StatusConditions.NORMAL) user.Total.RollBonus += 2;
                    break;
                case 189: // thief
                    if (opponent.Pokemon.AttachCard != null)
                    {
                        user.Pokemon.AttachCard = opponent.Pokemon.AttachCard;
                        Console.WriteLine(user + "stole the opponent’s item card " + opponent.Pokemon.AttachCard.Name);
                        opponent.Pokemon.AttachCard = null;
                    }
                    break;
                case 199: // incineroar
                    if (opponent.Pokemon.AttachCard != null)
                    {
                        BattlerPlayer player = (BattlerPlayer)opponent;
                        BattleLog.AddLog(player.TrainerName + "’s " + opponent.Pokemon.AttachCard + " was disabled in this battle by " + user.GetUsedMoveName());
                        opponent.Pokemon.AttachCard = null;
                    }
                    break;
                case 202: // acrobatics
                    if (user.Pokemon.AttachCard == null)
                    {
                        user.Total.RollBonus += 1;
                    }
                    break;
                case 203: // embargo
                    opponent.BuffsAndDebuffs.Add(EffectType.NOITEM);
                    opponent.Pokemon.AttachCard = null;
                    BattleLog.AddLog(user.Pokemon.GetName() + " disabled all of the opponent’s cards.");
                    break;
                case 205: //playback
                    if (opponent.Pokemon.AttachCard != null || opponent.UsedCard != null)
                    {
                        user.Total.RollBonus += 1;
                    }
                    break;
                case 206: // retaliate
                    break;
                case 223: // Nature Power
                    {
                        BattleLog.AddLog("Nature Power changed into a new move...");

                        // Lista dos possíveis moves correspondentes ao resultado do dado (1–8)
                        int[] moveIds = { 224, 70, 18, 4, 111, 109, 63, 47 };

                        int roll = DiceRollService.RollD8();
                        int selectedMoveId = moveIds[roll - 1]; // índice ajustado (0–7)

                        user.SetUsedMove(DataLists.GetMoveID(selectedMoveId));

                        BattleLog.AddLog($"The move became {user.GetUsedMoveName}");
                        break;
                    }
                default: break;
            }
        }
    }
}
