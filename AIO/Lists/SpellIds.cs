namespace AIO.Lists
{
    public static class SpellIds
    {
        public static class Paladin
        {
            public const int GreaterBlessingOfMight = 19742;
            public const int DivinePlea = 54428;
            public const int HandOfFreedom = 1044;
            public const int DivineProtection = 498;
            public const int SacredShield = 53601;
            public const int Purify = 1152;
            public const int FlashOfLight = 19750;
            public const int HammerOfJustice = 853;
            public const int HammerOfWrath = 24275;
            public const int HolyLight = 635;
            public const int HandOfReckoning = 62124;
            public const int AvengersShield = 31935;
            public const int AvengingWrath = 31884;
            public const int JudgementOfLight = 20271;
            public const int JudgementOfWisdom = 53408;
            public const int DivineStorm = 53385;
            public const int CrusaderStrike = 35395;
            public const int Consecration = 26573;
            public const int Exorcism = 879;
            public const int HolyWrath = 2812;
            public const int TheArtOfWar = 53486; // Buff ID
            public const int AutoAttack = 6603; // Auto Attack ID might vary or not work as Spell but standard "Auto Attack" string usually works manually. RotationSpell usually wraps a real spell. Auto Attack is a bit special.
            // "Auto Attack" is usually just cast by name "Auto Attack" in English. In Chinese it is "自动攻击".
            // Since it is special, we might need to check if 6603 works or if we need GetSpellInfo(6603).
        }


        public static class Rogue
        {
            public const int AutoAttack = 6603;
            public const int Sprint = 11305;
            public const int Kick = 1766;
            public const int Evasion = 26669;
            public const int Riposte = 14251;
            public const int BladeFlurry = 13877;
            public const int AdrenalineRush = 13750;
            public const int SliceAndDice = 5171;
            public const int Eviscerate = 2098;
            public const int KillingSpree = 51690;
            public const int SinisterStrike = 1752;
            public const int Stealth = 1784;
            public const int Feint = 1966;
            public const int CloakOfShadows = 31224;
            public const int Blind = 2094;
            public const int FanOfKnives = 51723;
            public const int Vanish = 26889; 
            public const int ColdBlood = 14177;
            public const int Envenom = 32645;
            public const int HungerForBlood = 51662;
            public const int Rupture = 1943;
            public const int Mutilate = 1329;
            public const int Overkill = 58427; 

        }

        public static class Warrior
        {
            public const int AutoAttack = 6603;
            public const int BattleStance = 2457;
            public const int DefensiveStance = 71;
            public const int BerserkerStance = 2458;
            public const int Charge = 11578;
            public const int Intercept = 20252;
            public const int Pummel = 6552;
            public const int Whirlwind = 1680;
            public const int Slam = 1464;
            public const int Hamstring = 1715;
            public const int Bloodrage = 2687;
            public const int BerserkerRage = 18499;
            public const int Bloodthirst = 23881;
            public const int DeathWish = 12292;
            public const int Recklessness = 1719;
            public const int Execute = 5308;
            public const int VictoryRush = 34428;
            public const int Rend = 772;
            public const int ThunderClap = 6343;
            public const int HeroicStrike = 78;
            public const int MortalStrike = 12294;
            public const int Bladestorm = 46924;
            public const int SweepingStrikes = 12328;
            public const int Overpower = 7384;
            public const int ShatteringThrow = 64382;
            public const int Retaliation = 20230;
            public const int IntimidatingShout = 5246;
            public const int DemoralizingShout = 1160;
            public const int PiercingHowl = 12323;
            public const int ShieldBash = 72;
            public const int LastStand = 12975;
            public const int ShieldWall = 871;
            public const int ShieldBlock = 2565;
            public const int EnragedRegeneration = 55694;
            public const int SpellReflection = 23920;
            public const int ChallengingShout = 1161;
            public const int MockingBlow = 694;
            public const int Taunt = 355;
            public const int ShieldSlam = 23922;
            public const int Shockwave = 46968;
            public const int Revenge = 6572;
            public const int Devastate = 20243;
            public const int Cleave = 845;
            public const int ConcussionBlow = 12809;
            public const int Vigilance = 50720;
            public const int Intervene = 3411;
            public const int Disarm = 676;
            public const int SunderArmor = 7386;
            public const int SunderArmor = 7386;
            public const int BattleShout = 6673;
            public const int CommandingShout = 469;
        }

        public static class DeathKnight
        {
            public const int AutoAttack = 6603;
            public const int RaiseDead = 46584;
            public const int MindFreeze = 47528;
            public const int Strangulate = 47476;
            public const int DeathAndDecay = 43265;
            public const int IcyTouch = 45477;
            public const int PlagueStrike = 45462;
            public const int SummonGargoyle = 49206;
            public const int Pestilence = 50842;
            public const int BloodStrike = 45902;
            public const int HeartStrike = 55050;
            public const int BloodBoil = 48721;
            public const int DeathStrike = 49998;
            public const int ScourgeStrike = 55090;
            public const int DeathCoil = 47541;
            public const int HowlingBlast = 49184;
            public const int IceboundFortitude = 48792;
            public const int FrostStrike = 49143;
            public const int Obliterate = 49020;
            public const int DarkCommand = 56222;
            public const int DeathGrip = 49576;
            public const int AntiMagicShell = 48707;
            public const int VampiricBlood = 55233;
            public const int RuneTap = 48982;
            public const int EmpowerRuneWeapon = 47568;
            public const int ChainsOfIce = 45524;
            public const int BloodTap = 45529;
            public const int MarkOfBlood = 49005;
            public const int DancingRuneWeapon = 49028;
        }

        public static class Druid
        {
            public const int AutoAttack = 6603;
            public const int RemoveCurse = 2782;
            public const int HealingTouch = 5185;
            public const int Berserk = 50334;
            public const int Bash = 5211;
            public const int DemoralizingRoar = 99;
            public const int SwipeBear = 779;
            public const int Growl = 6795;
            public const int Maul = 6807;
            public const int Moonfire = 8921;
            public const int Wrath = 5176;
            public const int Regrowth = 8936;
            public const int FaerieFireFeral = 16857;
            public const int Enrage = 5229;
            public const int MangleBear = 33878;
            public const int Prowl = 5215;
            public const int FeralChargeCat = 49376;
            public const int Pounce = 9005;
            public const int Ravage = 6785;
            public const int Dash = 1850;
            public const int Rake = 1822;
            public const int Rip = 1079;
            public const int FerociousBite = 22568;
            public const int MangleCat = 33876;
            public const int Claw = 1082;
            public const int MoonkinForm = 24858;
            public const int AbolishPoison = 2893;
            public const int Starfall = 48505;
            public const int Typhoon = 50516;
            public const int ForceOfNature = 33831;
            public const int Hurricane = 16914;
            public const int Starfire = 2912;
            public const int InsectSwarm = 5570;
            public const int FaerieFire = 770;
            public const int Rejuvenation = 774;
            public const int Barkskin = 22812;
            public const int Innervate = 29166;
            public const int Lifebloom = 33763;
            public const int WildGrowth = 48438;
            public const int Nourish = 50464;
            public const int Swiftmend = 18562;
            public const int TreeOfLife = 33891;
            public const int NaturesGrasp = 16689;
        }

        public static class Hunter
        {
            public const int AutoAttack = 6603;
            public const int HuntersMark = 1130;
            public const int KillCommand = 34026;
            public const int SerpentSting = 1978;
            public const int ArcaneShot = 3044;
            public const int MultiShot = 2643;
            public const int BlackArrow = 3674;
            public const int ExplosiveShot = 53301;
            public const int AimedShot = 19434;
            public const int SteadyShot = 56641;
            public const int RaptorStrike = 2973;
            public const int Disengage = 781;
            public const int FeignDeath = 5384;
            public const int SilencingShot = 34490;
            public const int Readiness = 23989;
            public const int KillShot = 53351;
            public const int Volley = 1510;
            public const int ViperSting = 3034;
            public const int ChimeraShot = 53209;
            public const int Deterrence = 19263;
            public const int Intimidation = 19577;
            public const int ConcussiveShot = 5116;
            public const int Misdirection = 34477;
            public const int BestialWrath = 19574;
            public const int RapidFire = 3045;
        }

        public static class Mage
        {
            public const int AutoAttack = 6603;
            public const int Shoot = 5019; // Shoot Wand
            public const int Polymorph = 118;
            public const int IcyVeins = 12472;
            public const int ArcanePower = 12042;
            public const int MirrorImage = 55342;
            public const int PresenceOfMind = 12043;
            public const int ArcaneMissiles = 5143;
            public const int ArcaneBlast = 30451;
            public const int FireBlast = 2136;
            public const int Fireball = 133;
            public const int Frostbolt = 116;
            public const int ManaShield = 1463;
            public const int FrostNova = 122;
            public const int IceBlock = 45438;
            public const int ColdSnap = 11958;
            public const int Counterspell = 2139;
            public const int ConeOfCold = 120;
            public const int Blizzard = 10;
            public const int FrostfireBolt = 44614;
            public const int Evocation = 12051;
            public const int SummonWaterElemental = 31687;
            public const int DeepFreeze = 44572;
            public const int IceLance = 30455;
            public const int Pyroblast = 11366;
            public const int LivingBomb = 44457;
            public const int Flamestrike = 2120;
            public const int Scorch = 2948;
            public const int Combustion = 11129;
            public const int BlastWave = 11113;
            public const int DragonsBreath = 31661;
            public const int Invisibility = 66;
            public const int IceBarrier = 11426;
        }

        public static class Priest
        {
            public const int AutoAttack = 6603;
            public const int Shoot = 5019;
            public const int Smite = 585;
            public const int PowerWordShield = 17;
            public const int Renew = 139;
            public const int PsychicScream = 8122;
            public const int Shadowfiend = 34433;
            public const int Dispersion = 47585;
            public const int Shadowform = 15473;
            public const int MindSear = 48045;
            public const int VampiricTouch = 34914;
            public const int DevouringPlague = 2944;
            public const int MindBlast = 8092;
            public const int ShadowWordPain = 589;
            public const int MindFlay = 15407;
            public const int VampiricEmbrace = 15286;
            public const int LesserHeal = 2050;
            public const int FlashHeal = 2061;
            public const int InnerFocus = 14751;
            public const int ShadowWordDeath = 32379;
            public const int CureDisease = 528;
            public const int AbolishDisease = 552;
            public const int DispelMagic = 527;
            public const int GuardianSpirit = 47788;
            public const int CircleOfHealing = 34861;
            public const int Penance = 47540;
            public const int PrayerOfHealing = 596;
            public const int PrayerOfMending = 33076;
            public const int DivineHymn = 64843;
            public const int HymnOfHope = 64901;
            public const int PainSuppression = 33206;
        }

        public static class Shaman
        {
            public const int AutoAttack = 6603;
            public const int HealingWave = 331;
            public const int FeralSpirit = 51533;
            public const int CureToxins = 526;
            public const int LightningBolt = 403;
            public const int ChainLightning = 421;
            public const int FireNova = 1535;
            public const int ShamanisticRage = 30823;
            public const int WindShear = 57994;
            public const int Stormstrike = 17364;
            public const int FlameShock = 8050;
            public const int EarthShock = 8042;
            public const int LavaLash = 60103;
            public const int LavaBurst = 51505;
            public const int Riptide = 61295;
            public const int LesserHealingWave = 8004;
            public const int CleanseSpirit = 51886;
            public const int ChainHeal = 1064;
            public const int WaterShield = 52127;
            public const int EarthShield = 974;
            public const int Bloodlust = 2825;
            public const int Heroism = 32182;
            public const int Thunderstorm = 51490;
            public const int Hex = 51514;
        }

        public static class Warlock
        {
            public const int AutoAttack = 6603;
            public const int Shoot = 5019;
            public const int DrainSoul = 1120;
            public const int DeathCoil = 6789;
            public const int LifeTap = 1454;
            public const int CurseOfTheElements = 1490;
            public const int Shadowfury = 30283;
            public const int RainOfFire = 5740;
            public const int DrainLife = 689;
            public const int Corruption = 172;
            public const int Immolate = 348;
            public const int Conflagrate = 17962;
            public const int ChaosBolt = 50796;
            public const int Incinerate = 29722;
            public const int ShadowBolt = 686;
            public const int Metamorphosis = 47241;
            public const int DemonicEmpowerment = 47193;
            public const int SeedOfCorruption = 27243;
            public const int HealthFunnel = 755;
            public const int CurseOfAgony = 980;
            public const int UnstableAffliction = 30108;
            public const int Haunt = 48181;
            public const int CurseOfWeakness = 702;
            public const int Fear = 5782;
            public const int HowlOfTerror = 5484;
            public const int SoulShatter = 29858;
            public const int Hellfire = 1949;
            public const int SoulFire = 6353;
        }
    }
}
