using System;
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace TextRPG
{


    public class Player                                                                     //플레이어 관련 데이터 클래스
    {
        public int Level { get; set; }
        public string Name { get; }
        public string Job { get; set; }
        public int Attack { get; set; }
        public int Deffense { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public int BonusAttack { get; set; }                                              // 장비의 공격력
        public int BonusDeffense { get; set; }                                            // 장비의 방어력
        public int ResAttack => Attack + ((Level-1)/2) + BonusAttack;                    //최종 공격력
        public int ResDeffense => Deffense +(Level-1) + BonusDeffense;                    //최종 방어력
        public int? EquipedLance {  get; set; }                                           // 장비한 무기의 아이템 ID 저장 (없을 시 null)
        public int? EquipedAmor {  get; set; }                                            // 장비한 방어구의 아이템 ID 저장 (없을 시 null)
        public int ClearedDungeon {get; set;}                                             // 클리어한 던전의 수 (레벨 업 시 0으로 초기화)

        public Player (string name)
        {
            Level = 1;
            Name = name;
            Job = "전사";
            Attack = 10;
            Deffense = 5;
            Health = 100;
            Gold = 1500;
            BonusAttack = 0;
            BonusDeffense = 0;
            EquipedLance = null;
            EquipedAmor = null;
            ClearedDungeon = 0;
        }

    }

    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; }
        public int Attack { get; set; }
        public int Deffense { get; set; }
        public string Info { get; set; }
        public int Price { get; set; }
        public int Type { get; set; }   // 1: 창 2: 갑옷
        public bool IsEquiped { get; set; } 
        public bool IsBought { get; set; }

        public Item (int itemId, String name, int attack, int deffence, string info, int price, int type)
        {
            ItemId = itemId;
            Name = name;
            Attack = attack;
            Deffense= deffence;
            Info = info;
            Price = price;
            Type = type;
            IsEquiped = false;
            IsBought = false;
            
        }

        public void Equip (Player player)                                                   //장비를 장착하는 메서드
        {
            if (player.EquipedLance == ItemId)                                              //이미 장착한 장비라면 장착 해제
            {                                                                               // 
                player.EquipedLance = null;                                                 //장착 한 장비가 아니라면 아이템의 타입 확인 후 장비를 장착한다.
                player.BonusAttack = 0;
            }

            else if (player.EquipedAmor == ItemId)
            {
                player.EquipedAmor = null;
                player.BonusDeffense = 0;
            }

            else if (Type ==1)
            {
                player.EquipedLance = ItemId;
                player.BonusAttack = Attack;
            }

            else if (Type == 2)
            {
                player.EquipedAmor = ItemId;
                player.BonusDeffense = Deffense;
            }

        }

        public void Sell (Player player)                                                       //아이템을 판매하는 메서드 
        {
            IsBought= false;                                                                   //구입 상태를 false로 초기화 
            if(ItemId ==player.EquipedLance)                                                   // 장비한 아이템이라면 장착을 해제함
            {
                player.EquipedLance = null;
                player.BonusAttack = 0;
            }
            else if (ItemId ==player.EquipedAmor)
            {
                player.EquipedAmor = null;
                player.BonusDeffense = 0;
            }

            player.Gold += (Price * 85) / 100;
                

                    
        }

    }





    internal class Program
    {
        public static string path = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {

            Player player = new Player("Default");
            Dictionary<int, Item> item = new Dictionary<int, Item>();

            int sel = 0;                                                              // 선택지 선택을 위한 변수
            LoadData(ref player, ref item);                                           // 게임 시작시 데이터 로드
                                                                                      // 저장 데이터가 없다면 캐릭터 생성
                                                                                      // 저장 데이터가 존재한다면 저장 데이터를 불러온다.






            while (true)
            {
                SaveData(player, item);                                               // 선택지 행동이 끝나고 다시 마을로 돌아왔을 때 데이터 저장 
                Village(player);                                                      // 플레이어와 아이템들의 정보가 저장 된다.

                sel = NumSel(5);                                                      // 0~5까지의 입력을 받는 메서드

                switch (sel)
                {
                    case 0:                                                           // 이번 선택지에선 0번 입력이 없기 때문에 따로 0번 선택지를 만들어 둠
                        Console.WriteLine("잘못된 입력입니다.");     
                        Thread.Sleep(1000);
                        break;
                    case 1:                                                          // 상태 보기
                        StatusView(player);
                        break;

                    case 2:
                        Invenotory(player,item);                                     // 인벤토리
                        break;

                    case 3:
                        Shop(player,item);                                           //상점
                        break;
                    case 4:
                        TeaTime(player);                                             //휴식
                        break;
                    case 5:
                        Entrance(player);                                            // 던전입장
                        break;

                }
            }

        }

        static int NumSel (int x)                    // 0~x 까지의 선택지를 선택하는 메서드
        {
            int a = 0;
          
            Console.Write("원하시는 행동을 입력해 주세요 : ");

            while (true)
            {
                try                                 //숫자 이외의 입력을 받았을 시의 예외처리
                {
                    a = int.Parse(Console.ReadLine());
                }

                catch (System.FormatException)                 // 숫자이외의 입력을 받았을 시 오류 메세지를 전송하고 다시 입력을 받음
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    continue;
                }

                if ((0<=a)&& (a<=x))              //입력받은 숫자가 선택지에 올바른지 검사
                {
                    break;                        //올바를 시 종료
                }

                else
                {
                    Console.WriteLine("잘못된 입력입니다.");        // 선택지 범위에 맞지 않은 숫자 입력을 받았을 시 오류 메세지 전송하고 다시 입력 받음
                }

            }

            return a;                                //입력받은 값 반환

        }

        static public void Village(Player player)                                       // 마을에선 선택지를 보여주기만 함
        {
            
            Console.Clear();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 휴식하기");
            Console.WriteLine("5. 던전 입장");


        }

        static public void StatusView(Player player)                                     // 스테이터스 확인 메서드
        {
            Console.Clear();
            Console.WriteLine($"이름 : {player.Name}");
            Console.WriteLine($"LV : {player.Level}");
            Console.WriteLine($"직업 : {player.Job}");
            Console.WriteLine($"공격력 : {player.ResAttack} (+{player.BonusAttack})");               
            Console.WriteLine($"방어력 : {player.ResDeffense} (+{player.BonusDeffense})");
            Console.WriteLine($"체력 : {player.Health}");
            Console.WriteLine($"GOLD : {player.Gold}");
            Console.WriteLine("");
            Console.WriteLine("0.나가기");
            NumSel(0);                                                                  // 0만을 선택지 입력으로 받음.
                                                                                        // 0을 입력으로 받으면 메서드가 종료되고 main에서는 반복문으로 인해 데이터 저장 후 다시 village가 동작함.

        }

        static public void Invenotory(Player player, Dictionary<int,Item> item)          // 인벤토리 
        {
            Console.Clear();
        
            foreach (KeyValuePair<int, Item> pair in item)
            {
                if (pair.Value.IsBought)
                {
                    if ((pair.Value.ItemId == player.EquipedLance) || (pair.Value.ItemId == player.EquipedAmor))          // 장착된 창/방어구인지 검사
                    {
                        Console.Write("[E]");
                    }
                    Console.Write($"- {pair.Value.Name,-9}ㅣ ");
                    if (pair.Value.Type == 1)                                                                            // 무기면 공격력, 방어구면 방어력을 표시해줌
                    {
                        Console.Write($"공격력+{pair.Value.Attack}  l");
                    }
                    else if (pair.Value.Type == 2)
                    {
                        Console.Write($"방어력+{pair.Value.Deffense}  l");
                    }
                    Console.WriteLine($"  {pair.Value.Info,-20}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착 관리");

            if(NumSel(1)==1)
            {
                InventoryEquip(player, item);
            }
        }

        static void InventoryEquip(Player player, Dictionary<int, Item> item)
        {
            Console.Clear();
            int num = 0;
            int[] arr= new int[item.Count];                                                                           
            foreach (KeyValuePair<int, Item> pair in item)
            {
                if (pair.Value.IsBought)                                                                                 //구입한 아이템만 표시
                {
                    
                    if ((pair.Value.ItemId == player.EquipedLance) ||(pair.Value.ItemId == player.EquipedAmor))          // 장착된 창/방어구인지 검사
                    {
                        Console.Write("[E]");
                    }
                    Console.Write($" {++num}.{pair.Value.Name,-9}ㅣ ");
                    arr[num] = pair.Value.ItemId;                                                                        //각 번호가 나타내는 아이템의 ID를 배열로 저장
                    if (pair.Value.Type == 1)
                    {
                        Console.Write($"공격력+{pair.Value.Attack}  l");
                    }
                    else if (pair.Value.Type == 2)
                    {
                        Console.Write($"방어력+{pair.Value.Deffense}  l");
                    }
                    Console.WriteLine($"  {pair.Value.Info,-20}");
                }
            
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            int sel = NumSel(num);
            if (sel != 0)                                                                                                // 나가기를 선택하지 않으면 선택한 아이템을 판매
            {
                item[arr[sel]].Equip(player);
                InventoryEquip(player, item);                                                                            // 판매 후 다시 장착관리로 돌아옴

            }
            
        }

        static public void Shop(Player player,Dictionary<int,Item> item)                                                 //상점
        {
            Console.Clear();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine(player.Gold + " G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach (KeyValuePair<int,Item> pair in item)
            {
                Console.Write($"{pair.Value.Name,-9}ㅣ ");
                if(pair.Value.Type ==1)
                {
                    Console.Write($"공격력+{pair.Value.Attack}  l");
                }
                else if (pair.Value.Type == 2)
                {
                    Console.Write($"방어력+{pair.Value.Deffense}  l");
                }
                Console.WriteLine($"  {pair.Value.Info,-20}  l " + (pair.Value.IsBought ? "판매완료" : pair.Value.Price + "G") );   // 삼항 연산자로 구입한 아이템인지 확인후 문구를 결정

            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");

            int a = NumSel(2);
            
            if (a==1)
            {
                ShopBuy(player,item);
            }
            else if (a==2)
            {
                ShopSell(player,item);
            }

        }

        static void ShopBuy(Player player, Dictionary<int,Item> item)                    //아이템 구매 
        {
            Console.Clear();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine(player.Gold + " G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach (KeyValuePair<int, Item> pair in item)
            {
                Console.Write($"{pair.Value.ItemId+1}. {pair.Value.Name,-9}ㅣ ");
                if (pair.Value.Type == 1)
                {
                    Console.Write($"공격력+{pair.Value.Attack}  l");
                }
                else if (pair.Value.Type == 2)
                {
                    Console.Write($"방어력+{pair.Value.Deffense}  l");
                }
                Console.WriteLine($"  {pair.Value.Info,-20}  l " + (pair.Value.IsBought ? "판매완료" : pair.Value.Price) + " G");
            }
            Console.WriteLine();
            Console.WriteLine("0.나가기");
           
                int x = NumSel(item.Count);

            if (x != 0)
            {
                Buy(x, item, player);
                ShopBuy(player, item);
            }
            
        }
        static void ShopSell(Player player, Dictionary<int,Item> item)                   //아이템 판매 
        {
            Console.Clear();

            int num = 0;
            int[] arr = new int[item.Count];
            Console.WriteLine("[보유 골드]");
            Console.WriteLine(player.Gold + " G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            foreach (KeyValuePair<int, Item> pair in item)
            {
                if (pair.Value.IsBought)                                                                                 //구입한 아이템만 표시
                {

                    if ((pair.Value.ItemId == player.EquipedLance) || (pair.Value.ItemId == player.EquipedAmor))          // 장착된 창/방어구인지 검사
                    {
                        Console.Write("[E]");
                    }
                    Console.Write($" {++num}.{pair.Value.Name,-9}ㅣ ");
                    arr[num] = pair.Value.ItemId;                                                                        //각 번호가 나타내는 아이템의 ID를 배열로 저장
                    if (pair.Value.Type == 1)
                    {
                        Console.Write($"공격력+{pair.Value.Attack}  l");
                    }
                    else if (pair.Value.Type == 2)
                    {
                        Console.Write($"방어력+{pair.Value.Deffense}  l");
                    }
                    Console.WriteLine($"  {pair.Value.Info,-20}");
                }

            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            int sel = NumSel(num);
            if (sel != 0)
            {
                item[arr[sel]].Sell(player);
                ShopSell(player, item);

            }


        }

        static void Buy(int i, Dictionary<int,Item> item,Player player)
        {
     
            int id = i - 1;
            if (item[id].IsBought)
            {
                Console.Clear();
                Console.WriteLine("이미 구매한 아이템 입니다.");
                Console.WriteLine();
                Console.WriteLine("0.확인");
                NumSel(0);
            }
            else if (player.Gold < item[id].Price)
            {
                Console.Clear();
                Console.WriteLine("Gold가 부족합니다");
                Console.WriteLine();
                Console.WriteLine("0.확인");
                NumSel(0);
            }
            else if (player.Gold >= item[id].Price)
            {
                Console.Clear();
                Console.WriteLine("구매를 완료했습니다");
                item[id].IsBought = true;
                player.Gold -= item[id].Price;
                Console.WriteLine();
                Console.WriteLine("0.확인");
                NumSel(0);
            }
        }

        static public void TeaTime(Player player)                                                   //휴식하기
        {
            Console.Clear();
            Console.WriteLine("500G를 내면 휴식할 수 있습니다. (보유 골드 : " + player.Gold + ")");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 휴식하기");

            if (NumSel(1) == 1)
            {
                player.Gold -= 500;
                player.Health = 100;
            }
        }

        static public void Entrance(Player player)                                                   //던전 입장
        {
            Console.Clear();
            Console.WriteLine("던전에 입장합니다. 입장할 난이도를 선택해 주세요");
            Console.WriteLine();
            Console.WriteLine("1. 쉬운 던전   - 권장 방어도 5  (기본 보상 1000G) ");
            Console.WriteLine("2. 일반 던전   - 권장 방어도 11 (기본 보상 1700G) ");
            Console.WriteLine("3. 어려운 던전 - 권장 방어도 18 (기본 보상 2500G) ");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();

            int sel = NumSel(3);

            if(sel !=0)
            {
                Dungeon(player,sel);
            }



        }

        static public void Dungeon(Player player, int i)
        {
            int RecommandDeffense = 0;
            int Reward = 0;

            int damage;
            int money;

            switch (i)                                                                             //선택 난이도에 따라 권장 방어력과 보상이 달라짐
            {
                case 1:
                    RecommandDeffense = 5;
                    Reward = 1000;
                    break;
                case 2:
                    RecommandDeffense = 11;
                    Reward = 1700;
                    break;
                case 3:
                    RecommandDeffense = 18;
                    Reward = 2500;
                    break;               
            }

            if (player.ResDeffense < RecommandDeffense)                                           // 권장 방어력 보다 낮을 때
            {
                if (new Random().Next(0, 10) < 4)                                                 // 일정 확률로 던전 실패
                {
                    Console.WriteLine("던전 실패! 체력이 절반으로 감소합니다.");
                    player.Health /= 2;
                }
                else
                {
                    damage = new Random().Next(20, 36) + RecommandDeffense - player.ResDeffense;
                    money = (new Random().Next(player.ResAttack, 2 * player.ResAttack + 1) * Reward) / 1000 + Reward;
                    if (player.Health <= damage)
                    {
                        Console.WriteLine("체력이 0이 되었습니다. GAME OVER");                   //체력이 0이 되었을 대 게임 오버
                        Console.WriteLine();
                        File.Delete(path + "\\PlayerData.json");                                 //게임 오버 시 저장 데이터를 삭제한다
                        File.Delete(path + "\\ItemData.json");
                        Console.WriteLine("0.게임 종료");
                        NumSel(0);
                        Environment.Exit(0);                                                     //프로그램 종료
                    }
                    else
                    {

                        Console.WriteLine($"던전 성공! {damage}의 체력 피해를 입고 {money}G 를 획득했습니다.");
                        player.Health -= damage;
                        player.Gold += money;
                        player.ClearedDungeon++;

                        if (player.Level == player.ClearedDungeon)
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"레벨 업! {++player.Level}레벨이 되었습니다.");
                            player.ClearedDungeon = 0;
                        }
                    }
                }

            }
            else                                                                                   //권장 방어력 보다 높을때
            {
                damage = new Random().Next(20, 36) + RecommandDeffense - player.ResDeffense;
                money = (new Random().Next(player.ResAttack, 2 * player.ResAttack + 1) * Reward) / 1000 + Reward;

                if (player.Health <= damage)
                {
                    Console.WriteLine("체력이 0이 되었습니다. GAME OVER");
                    Console.WriteLine();
                    File.Delete(path + "\\PlayerData.json");
                    File.Delete(path + "\\ItemData.json");
                    Console.WriteLine("0.게임 종료");
                    NumSel(0);
                    Environment.Exit(0);
                }

                Console.WriteLine($"던전 성공! {damage}의 체력 피해를 입고 {money}G 를 획득했습니다.");
                player.Health -= damage;
                player.Gold += money;
                player.ClearedDungeon++;

                if (player.Level == player.ClearedDungeon)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"레벨 업! {++player.Level}레벨이 되었습니다.");
                    player.ClearedDungeon = 0;
                }
            }


            Console.WriteLine();
            Console.WriteLine("0.확인");
            NumSel(0);
        }

        public static void SaveData(Player player,Dictionary<int,Item> item)                                 //게임 데이터 저장 
        {
           string playerData = JsonConvert.SerializeObject(player);                                          //Json 파일로 데이터 저장
           string ItemData = JsonConvert.SerializeObject(item);
           File.WriteAllText(path + "\\PlayerData.json", playerData);
           File.WriteAllText(path + "\\ItemData.json", ItemData);
        }

        public static void LoadData(ref Player player,ref Dictionary<int, Item> item)                        //데이터 로드
        {
            if (!File.Exists(path + "\\PlayerData.json"))                                                    // 저장 파일이 존재하지 않을 시 캐릭터,아이템 생성 
            {
                Console.WriteLine("스파르타 던전에 오신 것을 환영합니다. 당신의 이름을 설정해 주세요");
                

                 player = new Player($"{Console.ReadLine()}");

                
                item.Add(0, new Item(0, "수련자 갑옷", 0, 5, "수련에 도움을 주는 갑옷입니다.", 1000, 2));
                item.Add(1, new Item(1, "무쇠 갑옷", 0, 9, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000, 2));
                item.Add(2, new Item(2, "스파르타의 갑옷", 0, 15, "스파르타 전사들이 사용했다는 전설의 갑옷입니다.", 3500, 2));
                item.Add(3, new Item(3, "낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검 입니다.", 600, 1));
                item.Add(4, new Item(4, "청동 도끼", 5, 0, "어디선가 사용됐던거 같은 도끼입니다", 1500, 1));
                item.Add(5, new Item(5, "스파르타의 창", 7, 0, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 3000, 1));
                item.Add(6, new Item(6, "다이아몬드 창", 10, 0, "다이아몬드로 만들어져 가격이 비쌉니다.", 7000, 1));


            }
            else                                                                                           // 저장 파일이 존재할 시 저장 파일을 불러옴
            {
                string playerLoadData = File.ReadAllText(path + "\\PlayerData.json");
                player = JsonConvert.DeserializeObject<Player>(playerLoadData);

                string itemLoadData = File.ReadAllText(path + "\\ItemData.json");
                item = JsonConvert.DeserializeObject<Dictionary<int, Item>>(itemLoadData);
               
            }
        }
        

    }

   

}
