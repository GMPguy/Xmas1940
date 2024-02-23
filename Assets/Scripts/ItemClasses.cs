using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Purchasing;

public class ItemClasses : MonoBehaviour {

    // Categories
    public PlaneModel[] PlaneModels;
    public Engine[] EngineModels;
    public Special[] Specials;
    public Gun[] Guns;
    public Present[] Presents;
    public Special[] Additions;
    public Paint[] Paints;
    // Categories

    abstract public class Item {
        public GameScript GS;
        public int Price;
        public string[] Names;
        public string[] Desc;
        public int Category;
        public Item(int sCategory, int sPrice, string[] sNames, string[] sDesc){
            Category = sCategory; Price = sPrice; Names = sNames; Desc = sDesc;
        }
    }

    public class PlaneModel : Item{
        public int Class;
        public float Fuel, Health, Speed, RotationSpeed;
        public PlaneModel(int sCategory, int sPrice, string[] sNames, string[] sDesc, float sFuel, float sHealth, float sRotationSpeed, float sSpeed, int sClass) : base(sCategory, sPrice, sNames, sDesc){
            Fuel = sFuel; Health = sHealth; Speed = sSpeed; RotationSpeed = sRotationSpeed; Class = sClass;
            string[] pClasses = new string[]{"Fighter", "Bomber", "Striker", "Myśliwiec", "Bombowiec", "Szturmowiec"};
            Desc[0] = "Health: " + sHealth + "\nSpeed: " + sSpeed + "km/h\nTurning speed: " + sRotationSpeed + "\nClass: " + pClasses[sClass] + "\n\n" + sDesc[0]; 
            Desc[1] = "Zdrowie: " + sHealth + "\nPrędkość: " + sSpeed + "km/h\nPrędkość obrotowa: " + sRotationSpeed + "\nKlasa: " + pClasses[sClass+3] + "\n\n" + sDesc[1]; 
        }
    }

    public class Engine : Item{
        public float SpeedMultiplier;
        public Engine(int sCategory, int sPrice, string[] sNames, string[] sDesc, float sSpeedMultiplier) : base(sCategory, sPrice, sNames, sDesc){
            SpeedMultiplier = sSpeedMultiplier;
            Desc[0] = "Speed multiplier: x" + sSpeedMultiplier + "\n\n" + sDesc[0];
            Desc[1] = "Mnożnik prędkości: x" + sSpeedMultiplier + "\n\n" + sDesc[1];
        }
    }

    public class Special : Item{
        public float Cooldown;
        public Special(int sCategory, int sPrice, string[] sNames, string[] sDesc, float sCooldown) : base(sCategory, sPrice, sNames, sDesc){
            Cooldown = sCooldown;
            if(sCooldown > 0f){
                Desc[0] = "Cooldown: " + sCooldown + " seconds\n\n" + sDesc[0];
                Desc[1] = "Czas odnowienia: " + sCooldown + " sekund\n\n" + sDesc[1];
            }
        }
    }

    public class Gun : Item{
        public float Speed, Spread;
        public float[] Range, Damage, Cooldown;
        public Gun(int sCategory, int sPrice, string[] sNames, string[] sDesc, float[] sRanges, float[] sCooldown, float[] sDamage, float sSpeed, float sSpread) : base(sCategory, sPrice, sNames, sDesc){
            Range = sRanges; Damage = sDamage; Cooldown = sCooldown; Speed = sSpeed; Spread = sSpread;
            string strDamage = sDamage[0].ToString(); if (sDamage[0] != sDamage[1]) strDamage += "-" + sDamage[1].ToString();
            string strRange = sRanges[0].ToString(); if (sRanges[0] != sRanges[1]) strRange += "-" + sRanges[1].ToString();
            Desc[0] = "Damage: " + strDamage + "\nRange: " + strRange + "m\nFiring speed: " + sCooldown[0] + " seconds\nTime to overheat: " + sCooldown[1] + " seconds\nTime to cooldown: " + sCooldown[2] + " seconds\nTravel speed: " + Speed + " km/s\nGun spread: " + sSpread + "°\n\n" + sDesc[0];
            Desc[1] = "Obrażenia: " + strDamage + "\nDystans: " + strRange + "m\nPrędkość strzelania: " + sCooldown[0] + " sekund\nCzas do przegrzania: " + sCooldown[1] + " sekund\nCzas do schłodzenia: " + sCooldown[2] + " sekund\nPrędkość pocisków: " + Speed + " km/s\nRozrzut broni: " + sSpread + "°\n\n" + sDesc[1];
        }
    }

    public class Present : Item{
        public int Type; // 0 Cannon, 1 Homing, 2 Drop
        public float Speed, Range, Cooldown;
        public Present(int sCategory, int sPrice, string[] sNames, string[] sDesc, float sSpeed, float sRange, float sCooldown) : base(sCategory, sPrice, sNames, sDesc){
            Speed = sSpeed; Range = sRange; Cooldown = sCooldown;
            Desc[0] = "Travel speed: " + sSpeed + " km/h\nRange: " + sRange + "m\nFiring speed: " + sCooldown + " seconds\n\n" + sDesc[0];
            Desc[1] = "Prędkość pocisków: " + sSpeed + " km/h\nDystans: " + sRange + "m\nPrędkość strzelania: " + sCooldown + " seconds\n\n" + sDesc[1];
        }
    }

    public class Paint : Item{
        public Color32[] Paints;
        public Paint(int sCategory, int sPrice, string[] sNames, string[] sDesc, Color32[] nPaints) : base(sCategory, sPrice, sNames, sDesc){
            Paints = nPaints;
            Desc[0] = "First color: r" + nPaints[0].r + " g" + nPaints[0].g + " b" + nPaints[0].b + "\nSecond color: " + nPaints[1].r + " g" + nPaints[1].g + " b" + nPaints[1].b + "\n\n" + sDesc[0];
            Desc[1] = "Pierwszy kolor: r" + nPaints[0].r + " g" + nPaints[0].g + " b" + nPaints[0].b + "\nDrugi kolor: " + nPaints[1].r + " g" + nPaints[1].g + " b" + nPaints[1].b + "\n\n" + sDesc[1];
        }
    }

    void Start(){

        PlaneModels = new PlaneModel[]{
            new(0, 0,
                new string[]{"BP Mark.I", "BP Mark.I"},
                new string[]{"Desc", "Desc"},
                180f, 100f, 1f, 150f, 0),
            new(1, 0,
                new string[]{"BP Bomber V.I", "BP Bomber V.I"},
                new string[]{"Desc", "Desc"},
                200f, 200f, 0.75f, 100f, 1),
            new(2, 0,
                new string[]{"Monobird Striker", "Monobird Striker"},
                new string[]{"Desc", "Desc"},
                100f, 75f, 0.75f, 200f, 2),

            new(3, 0,
                new string[]{"Monobird B1", "Monobird B1"},
                new string[]{"Desc", "Desc"},
                100f, 75f, 0.75f, 200f, 1),
            new(4, 0,
                new string[]{"PukeFlame", "PukeFlame"},
                new string[]{"Desc", "Desc"},
                110f, 100f, 0.75f, 300f, 2),
            new(5, 0,
                new string[]{"Tornado", "Tornado"},
                new string[]{"Desc", "Desc"},
                200f, 150f, 1.25f, 180f, 0),

            new(5, 0,
                new string[]{"Bob", "Bob"},
                new string[]{"Desc", "Desc"},
                500f, 500f, 0.5f, 200f, 1),
            new(6, 0,
                new string[]{"Falcon G2", "Falcon G2"},
                new string[]{"Desc", "Desc"},
                200f, 200f, 1.5f, 200f, 0),
            new(7, 0,
                new string[]{"Falcon Dart", "Falcon Dart"},
                new string[]{"Desc", "Desc"},
                150f, 150f, 0.5f, 400f, 2),

            new(8, 0,
                new string[]{"SP Albatross", "SP Albatross"},
                new string[]{"Desc", "Desc"},
                500f, 750f, 0.35f, 250f, 1),
            new(9, 0,
                new string[]{"SP Arrow", "SP Arrow"},
                new string[]{"Desc", "Desc"},
                150f, 175f, 0.5f, 600f, 2),
            new(10, 0,
                new string[]{"SP Eagle", "SP Eagle"},
                new string[]{"Desc", "Desc"},
                250f, 250f, 2f, 225f, 0),

            new(11, 0,
                new string[]{"Stolen Messerschmitt", "Skradziony Messerschmitt"},
                new string[]{"Desc", "Desc"},
                200f, 150f, 1f, 180f, 0),
            new(12, 0,
                new string[]{"Stolen Messerschmitt 110", "Skradziony Messerschmitt 110"},
                new string[]{"Desc", "Desc"},
                100f, 100f, 1f, 200f, 2),
            new(13, 0,
                new string[]{"Stolen Messerschmitt Me 262", "Skradziony Messerschmitt Me 262"},
                new string[]{"Desc", "Desc"},
                100f, 100f, 0.5f, 400f, 2)
        };

        EngineModels = new Engine[]{
            new(0, 0,
                new string[]{"Basic Propeller", "Zwykłe śmigło"},
                new string[]{"Desc", "Desc"}, 1f),
            new(1, 0,
                new string[]{"Double Propeller", "Podwójne śmigło"},
                new string[]{"Desc", "Desc"}, 1.25f),
            new(2, 0,
                new string[]{"Jet Engine", "Silnik odrzutowy"},
                new string[]{"Desc", "Desc"}, 1.5f),
            new(3, 0,
                new string[]{"Double Jet Engine", "Podwójny silnik odrzutowy"},
                new string[]{"Desc", "Desc"}, 1.75f),
            new(4, 0,
                new string[]{"Magic Reindeer Dust", "Magiczny pył dla reniferów"},
                new string[]{"Desc", "Desc"}, 2f)
        };

        Specials = new Special[]{
            new(0, 0,
                new string[]{"None", "Brak"},
                new string[]{"Desc", "Desc"}, 0.1f),
            new(1, 0,
                new string[]{"Wrenches", "Klucze"},
                new string[]{"Desc", "Desc"}, 10f),
            new(2, 0,
                new string[]{"Fuel Tank", "Kanister z paliwem"},
                new string[]{"Desc", "Desc"}, 10f),
            new(3, 0,
                new string[]{"Homing Missile", "Pocisk samonaprowadzający"},
                new string[]{"Desc", "Desc"}, 30f),
            new(4, 0,
                new string[]{"Brick", "Cegła"},
                new string[]{"Desc", "Desc"}, 10f),
        };

        Guns = new Gun[]{
            new(0, 0,
                new string[]{"Vickers", "Vickers"},
                new string[]{"Desc", "Desc"},
                new float[]{400f, 400f},
                new float[]{0.1f, 3f, 5},
                new float[]{5f, 20f}, 1000f, 1f),
            new(1, 0,
                new string[]{"M2 Browning", "M2 Browning"},
                new string[]{"Desc", "Desc"},
                new float[]{600f, 600f},
                new float[]{0.075f, 3f, 5f},
                new float[]{15f, 20f}, 1250f, 0.5f),
            new(2, 0,
                new string[]{"M3 Browning", "M3 Browning"},
                new string[]{"Desc", "Desc"},
                new float[]{600f, 600f},
                new float[]{0.075f, 5f, 5f},
                new float[]{20f, 50f}, 1500f, 0.25f),
            new(3, 0,
                new string[]{"Cannon", "Armaty"},
                new string[]{"Desc", "Desc"},
                new float[]{300f, 300f},
                new float[]{0.2f, 5f, 6f},
                new float[]{50f, 100f}, 750f, 0f),
            new(4, 0,
                new string[]{"Flammable", "Pociski palące"},
                new string[]{"Desc", "Desc"},
                new float[]{400f, 400f},
                new float[]{0.1f, 3f, 5f},
                new float[]{5f, 15f}, 1000f, 0.5f),
            new(5, 0,
                new string[]{"Mugger Missiles", "Pociski kradnące"},
                new string[]{"Desc", "Desc"},
                new float[]{400f, 400f},
                new float[]{0.1f, 3f, 5f},
                new float[]{5f, 15f}, 1000f, 0.5f),
            new(6, 0,
                new string[]{"Flak", "Pociski przeciwlotnicze"},
                new string[]{"Desc", "Desc"},
                new float[]{750f, 1250f},
                new float[]{0.4f, 30f, 9999f},
                new float[]{100f, 200f}, 1000f, 0f),
            new(7, 0,
                new string[]{"Jet Gun", "Broń odrzutowców"},
                new string[]{"Desc", "Desc"},
                new float[]{1000f, 1000f},
                new float[]{0.05f, 7.5f, 5f},
                new float[]{20f, 50f}, 2000f, 0.25f),
            new(8, 0,
                new string[]{"Laser", "Laser"},
                new string[]{"Desc", "Desc"},
                new float[]{500f, 500f},
                new float[]{0.075f, 15f, 9999f},
                new float[]{5f, 20f}, 9999f, 0f),
            new(9, 0,
                new string[]{"Blue Laser", "Niebieski laser"},
                new string[]{"Desc", "Desc"},
                new float[]{1000f, 1000f},
                new float[]{0.075f, 15f, 9999f},
                new float[]{15f, 20f}, 9999f, 0f),
            new(10, 0,
                new string[]{"Paintball", "Paintball"},
                new string[]{"Desc", "Desc"},
                new float[]{600f, 600f},
                new float[]{0.1f, 5f, 10f},
                new float[]{5f, 15f}, 1000f, 1f),
            new(11, 0,
                new string[]{"Rocket", "Rakieta"},
                new string[]{"Desc", "Desc"},
                new float[]{600f, 600f},
                new float[]{0.4f, 15f, 9999f},
                new float[]{50f, 100f}, 1000f, 1f),
        };

        Presents = new Present[]{
            new(0, 0,
                new string[]{"Slingshot", "Slingshot"},
                new string[]{"Desc", "Desc"},
                400f, 400f, 10f),
            new(1, 0,
                new string[]{"Present Cannon", "Armata na prezenty"},
                new string[]{"Desc", "Desc"},
                1000f, 1000f, 10f),
            new(2, 0,
                new string[]{"Sniper Rifle", "Karabin snajperski"},
                new string[]{"Desc", "Desc"},
                10000f, 10000f, 30f),
            new(3, 0,
                new string[]{"Homing Present", "Samonaprowadzający prezent"},
                new string[]{"Desc", "Desc"},
                400f, 500f, 30f),
        };

        Additions = new Special[]{
            new(0, 0,
                new string[]{"None", "Brak"},
                new string[]{"Desc", "Desc"}, 0f),
            new(1, 0,
                new string[]{"Zoom", "Przybliżenie"},
                new string[]{"Desc", "Desc"}, 0f),
            new(2, 0,
                new string[]{"Boost", "Przyśpieszenie"},
                new string[]{"Desc", "Desc"}, 0f),
            new(3, 0,
                new string[]{"Damper", "Zderzak"},
                new string[]{"Desc", "Desc"}, 0f),
            new(4, 0,
                new string[]{"Turret", "Wieżyczka"},
                new string[]{"Desc", "Desc"}, 0f),
            new(5, 0,
                new string[]{"Ammo Pack", "Paczka z ammunicją"},
                new string[]{"Desc", "Desc"}, 0f),
        };

        Paints = new Paint[]{
            new(0, 0,
                new string[]{"Basic Paint", "Podstawowa farba"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(200, 200, 200, 255), new(200, 100, 100, 255)
                }),
            new(1, 0,
                new string[]{"Present Colors", "Kolory prezentowe"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(55, 200, 55, 255), new(200, 55, 55, 255)
                }),
            new(2, 0,
                new string[]{"Monochrome", "Monochromowy"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(100, 100, 100, 255), new(55, 55, 55, 255)
                }),
            new(3, 0,
                new string[]{"Night", "Noc"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(0, 100, 200, 255), new(5, 5, 55, 255)
                }),
            new(4, 0,
                new string[]{"War Paint", "Kamuflaż"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(100, 175, 0, 255), new(55, 75, 55, 255)
                }),
            new(5, 0,
                new string[]{"Toy Paint", "Zabawkowa farba"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(0, 100, 200, 255), new(200, 0, 0, 255)
                }),
            new(6, 0,
                new string[]{"Girly", "Dziewczęcy"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(200, 0, 200, 255), new(100, 0, 100, 255)
                }),
            new(7, 0,
                new string[]{"Black and White", "Czarnobiały"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(225, 225, 225, 255), new(5, 5, 5, 255)
                }),
            new(8, 0,
                new string[]{"Royal", "Szlachetny"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(0, 75, 255, 255), new(255, 255, 0, 255)
                }),
            new(9, 0,
                new string[]{"Aggressive", "Agresywny"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(200, 0, 0, 255), new(5, 5, 5, 255)
                }),
            new(10, 0,
                new string[]{"Desert", "Pustynny"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(255, 240, 220, 255), new(155, 115, 85, 255)
                }),
            new(11, 0,
                new string[]{"Rich", "Bogaty"},
                new string[]{"Desc", "Desc"},
                new Color32[]{ new(255, 255, 255, 255), new(255, 190, 0, 255)
                }),
        };

    }

    public Gun ReceiveGunType(string Name){
        for(int fg = 0; fg <= Guns.Length; fg++){
            if(fg == Guns.Length){
                Debug.LogError("No gun of name " + Name + " found!");
                return null;
            } else if (Guns[fg].Names[0] == Name) {
                return Guns[fg];
            }
        }

        return null;
    }

    public Present ReceivePresentType(string Name){
        for(int fg = 0; fg <= Presents.Length; fg++){
            if(fg == Presents.Length){
                Debug.LogError("No gun of name " + Name + " found!");
                return null;
            } else if (Presents[fg].Names[0] == Name) {
                return Presents[fg];
            }
        }
        
        return null;
    }

}
