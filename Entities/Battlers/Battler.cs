using ProjetoPokemon.Data;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;
using System.Runtime.CompilerServices;

namespace ProjetoPokemon.Entities.Battlers
{
    enum SetupBattle
    {
        Attacker,
        Defender,
    }
    internal class Battler
    {
        // fixos
        public ProfilePokemon Pokemon { get; set; }
        public List<Move> MovesPokemon { get; set; } = new List<Move>();
        public SetupBattle Setup { get; }

        private Move UsedMove { get; set; } = Move.Null();
        public ItemCard? UsedCard { get; set; }
        public Move? LastMove { get; set; } // torment setup effect
        public List<EffectType> BuffsAndDebuffs { get; set; } = new List<EffectType>();
        public TotalCalculationService Total { get; set; } = new TotalCalculationService();

        public Battler(ProfilePokemon profile, SetupBattle setup)
        {
            Pokemon = profile.Clone();
            CreateMoveList();
            Setup = setup;
        }

        // move setup
        public virtual Move SelectMove (Battler target) // random wild pokemon
        {
            List<Move> listMoves = GetMoveList();

            if (Pokemon.CanAttack == false || listMoves.Count == 0)
                return Move.Null();

            return RandomMove(listMoves);
        }
        public List<Move> GetMoveList() // filtra os moves
        {
            // Garante que há movimentos
            if (MovesPokemon == null || MovesPokemon.Count == 0){
                Console.WriteLine("Error in moves pokémon profile: " + Pokemon.Pokemon.Name);
                return new List<Move>();
            }

            // Remove Sleep Talk (ID 218)
            List<Move> moveList = MovesPokemon
                .Where(p => p.MoveID != 218 && p.CanUse)
                .ToList();

            // Se o Pokémon estiver sob efeito de Taunt, remove movimentos com poder 0
            if (BuffsAndDebuffs.Contains(EffectType.TAUNT))
                moveList = moveList.Where(p => p.Power > 0).ToList();
            return moveList;
        }
        private void CreateMoveList()
        {
            foreach (var move in Pokemon.Pokemon.Moves)
            {
                MovesPokemon.Add(move.Copy());
            }
            // inserir TM
        }

        #region(Get UsedMove)
        // set
        public void SetPower(int n)
        {
            UsedMove.ChangePower(n);
        }
        public void SetDiceSide(int n)
        {
            UsedMove.ChangeSide(n);
        }
        public void SetEffect(EffectManager effectType)
        {
            UsedMove.Effects.Add(effectType);
        }
        public void SetUsedMove(Move move)
        {
            if(move != null)
                UsedMove = move;
        }
        public Move RandomUsedMove()
        {
            List<Move> moves = GetMoveList();
            int i = DiceRollService.RollDice(1, moves.Count);
            return moves[i - 1];
        }
        public void RechargeMove()
        {
            if (UsedMove != null)
                UsedMove.RechargeMove();
        }
        // get
        public Move GetUsedMove()
        {
            return UsedMove;
        }
        public EffectManager? GetEffectUsedMove(EffectType effect)
        {
            if (UsedMove.Effects.Count == 0)
                return null;

            if (CheckEffectUsedMove(effect))
            {
                return UsedMove.Effects.Where(p => p.EffectType == effect).FirstOrDefault();
            }
            else return null;
        }
        public int GetUsedMoveID() { return UsedMove.MoveID; }
        public string GetUsedMoveName() { return UsedMove.Name; }
        public TypePokemon GetUsedMoveType() { return UsedMove.Type; }
        public int GetUsedMovePower() { return UsedMove.Power; }
        public int GetUsedMoveDiceSide()
        {
            return UsedMove.DiceSides;
        }
        // check
        public bool CheckEffectUsedMove(EffectType effect)
        {
            if (UsedMove.Effects.Count == 0) return false;
            return UsedMove.Effects.Any(p => p.EffectType == effect);
        }
        public bool CheckUsedMoveTargetEffect(char t)
        {
            return UsedMove.Effects.Any(p => p.TargetEffect == t);
        }
        public bool CheckEffectRoll(int redRoll)
        {
            if (UsedMove.EffectRoll > 0)
            {
                BattleLog.AddLog($"\n**{Pokemon.GetName()} {UsedMove.Name} roll effect: {redRoll} - {UsedMove.EffectRoll} to activate.");
                return UsedMove.EffectRoll <= redRoll;
            }
            return true;
        }
        public bool HasBuffEffect(EffectType effectType)
        {
            return BuffsAndDebuffs != null && BuffsAndDebuffs.Contains(effectType);
        }
        #endregion

        public bool CheckCondition()
        {
            // paralyzed
            if (Pokemon.Conditions == StatusConditions.PARALYZED)
            {
                if (!BattleConditions.ParalyzedRoll())
                {
                    BattleLog.AddLog(Pokemon + " is paralyzed and cannot attack!");
                    return false;
                }
            }
            // sleeping
            else if (Pokemon.Conditions == StatusConditions.SLEEP)
            {
               Pokemon.ConditionCount--; // sleep count -1
                // woke up
                if (Pokemon.ConditionCount == 0)
                { 
                    BattleLog.AddLog(Pokemon + " woke up!"); 
                    Pokemon.Conditions = StatusConditions.NORMAL; return true; 
                }
                // sleep talk
                else if (MovesPokemon.Any(p => p.MoveID == 218)) 
                {
                    UsedMove = RandomMove(GetMoveList()); // forçar o randomico
                    BattleLog.AddLog(Pokemon + $" is still sleeping and use {GetUsedMoveName()} with {DataLists.GetMoveID(218).Name}"); // sleep talk effect

                    return false;
                }
                else
                {
                    BattleLog.AddLog(Pokemon + " is still sleeping...");
                    return false;
                }
            }
            // frozen
            else if (Pokemon.Conditions == StatusConditions.FROZEN)
            {
                if (BattleConditions.FrozenRoll())
                {
                    Pokemon.Conditions = StatusConditions.NORMAL;
                    BattleLog.AddLog(Pokemon + " has been unfrozen and can attack!");
                    return true;
                }
                else BattleLog.AddLog(Pokemon + " is still frozen."); return false;
            }
            return true;
        }
        public virtual ProfilePokemon ChangeRandomPokemon()
        {
            return Pokemon;
        }
        private Move RandomMove(List<Move> listMoves)
        {
            if (listMoves.Count == 0)
                return Move.Null();

            Random rnd = new Random();
            return listMoves[rnd.Next(listMoves.Count)];
        }
        public override string ToString()
        {
            string buffs = string.Empty;
            if (BuffsAndDebuffs.Count > 0)
            {
                foreach (var buff in BuffsAndDebuffs)
                {
                    buffs += buff.ToString() + " ";
                }
                buffs = "[" + buffs.Substring(0, buffs.Length - 1) + "]";
            }
            string status = string.Empty;
            if (Pokemon.Conditions != StatusConditions.NORMAL) status = $"[{Pokemon.Conditions.ToString()}] ";
            return $"{status}{buffs}Wild {Pokemon.GetName()}";
        }
    }
}
