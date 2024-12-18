using System;
using System.Collections.Generic;
using System.IO;

namespace CarWave
{
    // Klasa reprezentująca auto
    class Auto
    {
        public string Marka { get; private set; }
        public string Model { get; private set; }
        public int RokProdukcji { get; private set; }
        public decimal CenaWynajmu { get; private set; }
        public decimal Kaucja { get; private set; }
        public string NumerRejestracyjny { get; private set; }
        public bool Wynajete { get; private set; }
        public bool WSerwisie { get; private set; }

        public Auto(string marka, string model, int rokProdukcji, decimal cenaWynajmu, decimal kaucja, string numerRejestracyjny)
        {
            Marka = marka;
            Model = model;
            RokProdukcji = rokProdukcji;
            CenaWynajmu = cenaWynajmu;
            Kaucja = kaucja;
            NumerRejestracyjny = numerRejestracyjny;
        }

        public void WynajmijAuto()
        {
            if (WSerwisie)
                throw new InvalidOperationException("Auto jest w serwisie i nie może być wynajęte.");

            if (Wynajete)
                throw new InvalidOperationException("Auto jest już wynajęte.");

            Wynajete = true;
        }

        public void ZwrocAuto()
        {
            Wynajete = false;
        }

        public void OznaczJakoWSerwisie()
        {
            WSerwisie = true;
            Wynajete = false;
        }

        public void OznaczJakoSprawne()
        {
            WSerwisie = false;
        }

        public void WyswietlInformacje()
        {
            Console.WriteLine($"Auto: {Marka} {Model} ({RokProdukcji}), Rejestracja: {NumerRejestracyjny}, Cena: {CenaWynajmu:C}, Kaucja: {Kaucja:C}, Wynajęte: {Wynajete}, W serwisie: {WSerwisie}");
        }

        // Metoda do zapisu auta do pliku
        public string ZapiszDoPliku()
        {
            return $"{Marka},{Model},{RokProdukcji},{CenaWynajmu},{Kaucja},{NumerRejestracyjny},{Wynajete},{WSerwisie}";
        }

        // Metoda do odczytu auta z pliku
        public static Auto OdczytajZPliku(string linia)
        {
            var dane = linia.Split(',');
            if (dane.Length == 8 &&
                int.TryParse(dane[2], out int rok) &&
                decimal.TryParse(dane[3], out decimal cenaWynajmu) &&
                decimal.TryParse(dane[4], out decimal kaucja) &&
                bool.TryParse(dane[6], out bool wynajete) &&
                bool.TryParse(dane[7], out bool wSerwisie))
            {
                var auto = new Auto(dane[0], dane[1], rok, cenaWynajmu, kaucja, dane[5]);
                if (wynajete) auto.WynajmijAuto();
                if (wSerwisie) auto.OznaczJakoWSerwisie();
                return auto;
            }
            throw new FormatException("Niepoprawny format danych auta.");
        }
    }

    // Klasa zarządzająca flotą
    class ZarzadzanieFlota
    {
        private List<Auto> auta = new List<Auto>();

        public void DodajAuto(Auto auto)
        {
            auta.Add(auto);
        }

        public void ListaWszystkichAut()
        {
            Console.WriteLine("\n--- Wszystkie Auta ---");
            foreach (var auto in auta)
                auto.WyswietlInformacje();
        }

        public IEnumerable<Auto> PobierzAutaDostepne()
        {
            foreach (var auto in auta)
                if (!auto.Wynajete && !auto.WSerwisie)
                    yield return auto;
        }

        public IEnumerable<Auto> PobierzAutaWynajete()
        {
            foreach (var auto in auta)
                if (auto.Wynajete)
                    yield return auto;
        }

        public IEnumerable<Auto> PobierzAutaWSerwisie()
        {
            foreach (var auto in auta)
                if (auto.WSerwisie)
                    yield return auto;
        }

        // Metoda do zapisu floty do pliku
        public void ZapiszFloteDoPliku(string sciezka)
        {
            using (StreamWriter writer = new StreamWriter(sciezka))
            {
                foreach (var auto in auta)
                {
                    writer.WriteLine(auto.ZapiszDoPliku());
                }
            }
        }

        // Metoda do odczytu floty z pliku
        public void OdczytajFloteZPliku(string sciezka)
        {
            if (File.Exists(sciezka))
            {
                using (StreamReader reader = new StreamReader(sciezka))
                {
                    string linia;
                    while ((linia = reader.ReadLine()) != null)
                    {
                        try
                        {
                            Auto auto = Auto.OdczytajZPliku(linia);
                            DodajAuto(auto);
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Błąd odczytu: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Plik nie istnieje.");
            }
        }
    }

    // Klasa zarządzająca finansami firmy
    class FinanseFirmy
    {
        public decimal PrzychodyZWynajmu { get; private set; }

        public void DodajPrzychod(decimal kwota)
        {
            PrzychodyZWynajmu += kwota;
        }

        public void WyswietlBilans()
        {
            Console.WriteLine($"\n--- Finanse Firmy ---\nPrzychody z wynajmu aut: {PrzychodyZWynajmu:C}");
        }
    }

    class Program
    {
        static string GenerujWarszawskaRejestracje()
        {
            Random random = new Random();
            string litery = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string liczby = random.Next(1000, 9999).ToString();
            string czescLiterowa = $"{litery[random.Next(litery.Length)]}{litery[random.Next(litery.Length)]}";
            return $"WI {czescLiterowa}{liczby}";
        }

        static void Main(string[] args)
        {
            ZarzadzanieFlota zarzadzanieFlota = new ZarzadzanieFlota();
            FinanseFirmy finanseFirmy = new FinanseFirmy();

            // Odczyt floty z pliku
            zarzadzanieFlota.OdczytajFloteZPliku("flota.txt");

            // Dodawanie aut
            zarzadzanieFlota.DodajAuto(new Auto("BMW", "M3 Competition", 2022, 900, 5000, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Mercedes", "AMG GT 63S", 2021, 1100, 7000, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Toyota", "Sienna", 2023, 400, 2000, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Ford", "Transit Custom", 2020, 300, 1500, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Tesla", "Model S Plaid", 2022, 1000, 9000, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Tesla", "Cybertruk", 2024, 1500, 15000, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Toyta", "Corolla", 2022, 400, 5000, GenerujWarszawskaRejestracje()));
            zarzadzanieFlota.DodajAuto(new Auto("Skoda ", "Fabia", 2022, 300, 2000, GenerujWarszawskaRejestracje()));

            Console.WriteLine("********************************");
            Console.WriteLine("*                              *");
            Console.WriteLine("*           CARWAVE            *");
            Console.WriteLine("*                              *");
            Console.WriteLine("********************************");

            Console.WriteLine("Login :");
            string login = Console.ReadLine();
            Console.WriteLine("Hasło:");
            string haslo = Console.ReadLine();

            if (login == "Grzegorz" && haslo == "35744")
            {
                Console.WriteLine($"Witaj, {login}!");

                bool dziala = true;
                while (dziala)
                {
                    Console.WriteLine("\n--- Menu Główne ---");
                    Console.WriteLine("1. Lista Wszystkich Aut");
                    Console.WriteLine("2. Lista Aut Dostępnych");
                    Console.WriteLine("3. Lista Aut Wynajętych");
                    Console.WriteLine("4. Lista Aut w Serwisie");
                    Console.WriteLine("5. Oznacz Auto jako Wynajęte");
                    Console.WriteLine("6. Oznacz Auto jako W Serwisie");
                    Console.WriteLine("7. Finanse Firmy");
                    Console.WriteLine("8. Zapisz Flotę do Pliku");
                    Console.WriteLine("9. Wyjdź");

                    string wybor = Console.ReadLine();

                    switch (wybor)
                    {
                        case "1":
                            zarzadzanieFlota.ListaWszystkichAut();
                            break;

                        case "2":
                            Console.WriteLine("\n--- Auta Dostępne ---");
                            foreach (var auto in zarzadzanieFlota.PobierzAutaDostepne())
                                auto.WyswietlInformacje();
                            break;

                        case "3":
                            Console.WriteLine("\n--- Auta Wynajęte ---");
                            foreach (var auto in zarzadzanieFlota.PobierzAutaWynajete())
                                auto.WyswietlInformacje();
                            break;

                        case "4":
                            Console.WriteLine("\n--- Auta w Serwisie ---");
                            foreach (var auto in zarzadzanieFlota.PobierzAutaWSerwisie())
                                auto.WyswietlInformacje();
                            break;

                        case "5":
                            Console.WriteLine("\n--- Oznacz Auto jako Wynajęte ---");
                            foreach (var auto in zarzadzanieFlota.PobierzAutaDostepne())
                                Console.WriteLine($"- {auto.Marka} {auto.Model} ({auto.NumerRejestracyjny})");

                            Console.Write("Podaj numer rejestracyjny auta: ");
                            string nrRejestracyjnyWynajem = Console.ReadLine();

                            try
                            {
                                foreach (var auto in zarzadzanieFlota.PobierzAutaDostepne())
                                {
                                    if (auto.NumerRejestracyjny.Equals(nrRejestracyjnyWynajem, StringComparison.OrdinalIgnoreCase))
                                    {
                                        auto.WynajmijAuto();
                                        finanseFirmy.DodajPrzychod(auto.CenaWynajmu);
                                        Console.WriteLine($"Auto {auto.Marka} {auto.Model} zostało oznaczone jako wynajęte. Dodano {auto.CenaWynajmu:C} do przychodów.");
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Błąd: {ex.Message}");
                            }
                            break;

                        case "6":
                            Console.WriteLine("\n--- Oznacz Auto jako W Serwisie ---");
                            foreach (var auto in zarzadzanieFlota.PobierzAutaDostepne())
                                Console.WriteLine($"- {auto.Marka} {auto.Model} ({auto.NumerRejestracyjny})");

                            Console.Write("Podaj numer rejestracyjny auta: ");
                            string nrRejestracyjnySerwis = Console.ReadLine();

                            try
                            {
                                foreach (var auto in zarzadzanieFlota.PobierzAutaDostepne())
                                {
                                    if (auto.NumerRejestracyjny.Equals(nrRejestracyjnySerwis, StringComparison.OrdinalIgnoreCase))
                                    {
                                        auto.OznaczJakoWSerwisie();
                                        Console.WriteLine($"Auto {auto.Marka} {auto.Model} zostało oznaczone jako w serwisie.");
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Błąd: {ex.Message}");
                            }
                            break;

                        case "7":
                            finanseFirmy.WyswietlBilans();
                            break;

                        case "8":
                            zarzadzanieFlota.ZapiszFloteDoPliku("flota.txt");
                            Console.WriteLine("Flota została zapisana do pliku.");
                            break;

                        case "9":
                            dziala = false;
                            break;

                        default:
                            Console.WriteLine("Nieprawidłowy wybór, spróbuj ponownie.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy login lub hasło.");
            }
        }
    }
}