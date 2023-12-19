using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 22)]
    public class Year2015Day22 : IAdventOfCode
    {
        //Credit for this solution goes to user Scarramanga who is 4th on the leader board.
        //Solution is simple and efficient. Some adjustments have to b e made for clarity
        //and details specific to my solution.

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        int bossHitpoints = 51;
        int bossDamage = 9;

        int costMissile = 53;
        int costDrain = 73;
        int costPoison = 173;
        int costShield = 113;
        int costRecharge = 229;

        const int startingHitPoints = 50;
        const int startingMana = 500;
        
        int playerArmour = 0;
        int cost = 0;

        int poisonTimer = 0;
        int rechargeTimer = 0;
        int shieldTimer = 0;

        int boss;
        int _player;
        int _mana;

        Random rand = new Random();

        enum ActionType {Nothing, Missile, Drain, Poison, Shield, Recharge}

        public Stopwatch _SW { get; set; }

        public Year2015Day22()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");


            _SW.Start();                       

            int answer = 9999999;

            for(int i = 0; i < 3000000; i++)
            {
                if(Fight())
                {
                    answer = Math.Min(answer, cost);
                }
            }

            
            _SW.Stop();

            Console.WriteLine("Part 1 - Least amount of Mana Spent to win fight: {0}, Execution Time: {1}", answer, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int answer2 = 9999999;

            for(int i = 0; i < 3000000; i++)
            {
                if(Fight(true))
                {
                    answer2 = Math.Min(answer2, cost);
                }
            }

            
            _SW.Stop();

            Console.WriteLine("Part 2 - Least amount of Mana Spent to win fight (Hard Mode): {0}, Execution Time: {1}", answer2, StopwatchUtil.getInstance().GetTimestamp(_SW));
        }   
        
        ActionType Choose()
        {

            if (_mana < costMissile)
            {
                return ActionType.Nothing;
            }

            while (true)
            {
                int next = rand.Next(5);
                if (next == 0 && _mana >= costMissile)
                {
                    return ActionType.Missile;
                }
                else if (next == 1 && _mana >= costDrain)
                {
                    return ActionType.Drain;
                }
                else if (next == 2 && _mana >= costPoison)
                {
                    return ActionType.Poison;
                }
                else if (next == 3 && _mana >= costRecharge)
                {
                    return ActionType.Recharge;
                }
                else if (next == 4 && _mana >= costShield)
                {
                    return ActionType.Shield;
                }
            }
        }


        bool Fight(bool hardMode = false)
        {
            bool turn = true;
            ActionType type = ActionType.Nothing;

            _player = startingHitPoints;

            boss = bossHitpoints;

            _mana = startingMana;
            cost = 0;
            poisonTimer = 0;
            rechargeTimer = 0;
            shieldTimer = 0;

            while (true)
            {
                if(poisonTimer > 0)
                {
                    poisonTimer--;
                    boss -= 3;
                }

                if(rechargeTimer > 0)
                {
                    rechargeTimer--;
                    _mana += 101;
                }

                if(shieldTimer > 0)
                {
                    shieldTimer--;
                }

                if(shieldTimer == 0)
                {
                    playerArmour = 0;
                }

                if (boss <= 0)
                {
                    return true;
                }

                if (_player <= 0)
                {
                    return false;
                }

                if (turn)
                {
                    //// hard mode

                    if (hardMode)
                    {
                        _player -= 1;
                        if (_player <= 0) return false;
                    }
                    
                    type = Choose();
                    if (type == ActionType.Nothing)
                    {
                        return false;
                    }
                    
                    if(type == ActionType.Drain)
                    {
                        boss -= 2;
                        _player += 2;
                        cost += costDrain;
                        _mana -= costDrain;
                    }
                    else if (type == ActionType.Missile)
                    {
                        boss -= 4;
                        cost += costMissile;
                        _mana -= costMissile;
                    }
                    else if (type == ActionType.Poison)
                    {
                        poisonTimer = 6;
                        cost += costPoison;
                        _mana -= costPoison;
                    }
                    else if (type == ActionType.Recharge)
                    {
                        rechargeTimer = 5;
                        cost += costRecharge;
                        _mana -= costRecharge;
                    }
                    else if (type == ActionType.Shield)
                    {
                        shieldTimer = 6;
                        playerArmour = 7;
                        cost += costShield;
                        _mana -= costShield;
                    }
                }
                else
                {
                    _player -= Math.Max(1, bossDamage - playerArmour);
                }

                turn = !turn;
            }
        }
    }
}
