using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;
using System.Linq;

namespace SafeRemover
{
    #region backup 01
    //class Program
    //{
    //    #region Description
    //    //Основная логика программы:

    //    //Получение подключенных съемных дисков с помощью метода GetRemovableDrives().

    //    //Программа проверяет, сколько съемных дисков подключено:
    //    //Если не найдено ни одного диска, программа выводит сообщение и завершает выполнение.
    //    //Если найден один диск, программа автоматически пытается его извлечь.
    //    //Если дисков несколько, программа выводит список и просит пользователя выбрать диск для извлечения или извлечь все диски.
    //    //Запрос у пользователя: если несколько дисков, программа выводит список подключенных флеш-дисков, предлагает выбрать, какой извлечь, или дает возможность извлечь все сразу.

    //    //Вывод ошибки при некорректном вводе: если пользователь вводит неправильное значение, программа выводит соответствующее сообщение.
    //    #endregion
    //    static void Main(string[] args)
    //    {
    //        Console.Title = "Безопасное удаление диска";

    //        DriveRemover remover = new DriveRemover();

    //        List<string> removableDrives = remover.GetRemovableDrives();

    //        if (removableDrives.Count == 0)
    //        {
    //            Console.WriteLine("Не подключено ни одного флеш-диска.");
    //            //return;
    //        }
    //        else if (removableDrives.Count == 1)
    //        {
    //            Console.WriteLine($"Обнаружен флеш-диск: {removableDrives[0]}");
    //            remover.SafelyRemoveDrive(removableDrives[0]);
    //        }
    //        else
    //        {
    //            Console.WriteLine("Обнаружены следующие флеш-диски:");

    //            for (int i = 0; i < removableDrives.Count; i++)
    //            {
    //                Console.WriteLine($"{i + 1}: {removableDrives[i]}");
    //            }

    //            Console.WriteLine("Введите номер флеш-диска для извлечения или 0 для извлечения всех:");
    //            int choice = int.Parse(Console.ReadLine());

    //            if (choice == 0)
    //            {
    //                foreach (var drive in removableDrives)
    //                {
    //                    remover.SafelyRemoveDrive(drive);
    //                }
    //            }
    //            else if (choice > 0 && choice <= removableDrives.Count)
    //            {
    //                remover.SafelyRemoveDrive(removableDrives[choice - 1]);
    //            }
    //            else
    //            {
    //                Console.WriteLine("Некорректный выбор.");
    //            }
    //        }

    //        Console.ReadKey();
    //    }
    //}

    //class DriveRemover
    //{
    //    #region Import_Win_API
    //    //Функция CreateFile из kernel32.dll используется для получения дескриптора файла или устройства.
    //    //В данном случае используется для открытия флеш-диска как устройства,
    //    //чтобы потом с ним можно было взаимодействовать
    //    //(например, отправлять команду на извлечение).
    //    #endregion
    //    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    //    private static extern IntPtr CreateFile(
    //        string lpFileName,
    //        uint dwDesiredAccess,
    //        uint dwShareMode,
    //        IntPtr lpSecurityAttributes,
    //        uint dwCreationDisposition,
    //        uint dwFlagsAndAttributes,
    //        IntPtr hTemplateFile);

    //    #region DeviceIoControl
    //    //Функция DeviceIoControl — это универсальный метод для управления устройствами, такими как диски.
    //    //С помощью этого метода отправляется команда на безопасное извлечение флеш-диска.
    //    //
    //    //Параметры:
    //    //hDevice — дескриптор устройства (возвращенный CreateFile).
    //    //dwIoControlCode — код команды, например, IOCTL_STORAGE_EJECT_MEDIA для извлечения диска.
    //    //lpInBuffer и lpOutBuffer — буферы для передачи данных (здесь они не нужны).
    //    #endregion
    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool DeviceIoControl(
    //        IntPtr hDevice,
    //        uint dwIoControlCode,
    //        IntPtr lpInBuffer,
    //        uint nInBufferSize,
    //        IntPtr lpOutBuffer,
    //        uint nOutBufferSize,
    //        ref uint lpBytesReturned,
    //        IntPtr lpOverlapped
    //        );

    //    #region CloseHandle
    //    //Функция CloseHandle закрывает дескриптор устройства после завершения работы с ним,
    //    //освобождая системные ресурсы.
    //    #endregion
    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool CloseHandle(IntPtr hObject);

    //    public List<string> GetRemovableDrives()
    //    {
    //        List<string> removableDrives = new List<string>();

    //        var allDrives = DriveInfo.GetDrives();

    //        foreach (var drive in allDrives)
    //        {
    //            if (drive.DriveType == DriveType.Removable && drive.IsReady)
    //            {
    //                removableDrives.Add(drive.Name);
    //            }
    //        }

    //        return removableDrives;
    //    }

    //    public void SafelyRemoveDrive(string driveLetter)
    //    {
    //        #region CONST_WINDOWS_API
    //        // Константы для Windows API
    //        //константы используются для взаимодействия с функциями Windows API. Например:

    //        //IOCTL_STORAGE_EJECT_MEDIA — код управления, который отправляется устройству для его безопасного извлечения.
    //        //FILE_SHARE_READ и FILE_SHARE_WRITE — указывают, что другие процессы могут иметь доступ к устройству на чтение и запись.
    //        //OPEN_EXISTING — используется при открытии существующего устройства(флеш - диска).
    //        //GENERIC_READ и GENERIC_WRITE — права доступа к устройству(чтение и запись). 
    //        #endregion
    //        const uint CodeEject = 0x2D4808;
    //        const uint FileShareRead = 0x00000001;
    //        const uint FileShareWrite = 0x00000002;
    //        const uint OpenExisting = 3;
    //        const uint GenericRead = 0x80000000;
    //        const uint GenericWrite = 0x40000000;

    //        string devicePath = @"\\.\" + driveLetter.TrimEnd('\\');
    //        IntPtr handle = CreateFile(devicePath, GenericRead | GenericWrite, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);

    //        if (handle == IntPtr.Zero)
    //        {
    //            Console.WriteLine($"Не удалось получить доступ к {driveLetter}. Ошибка: {Marshal.GetLastWin32Error()}");
    //            return;
    //        }

    //        uint bytesReturned = 0;
    //        bool result = DeviceIoControl(handle, CodeEject, IntPtr.Zero, 0, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);

    //        if (result)
    //        {
    //            Console.WriteLine($"Флеш-диск {driveLetter} успешно извлечен.");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Не удалось извлечь флеш-диск {driveLetter}. Ошибка: {Marshal.GetLastWin32Error()}");
    //        }

    //        CloseHandle(handle);
    //    }

    //}
    #endregion

    #region GPT
    //class DriveRemover
    //{
    //    private List<DriveInfo> _removableDrives;

    //    public DriveRemover()
    //    {
    //        _removableDrives = new List<DriveInfo>();
    //    }

    //    public void Start()
    //    {
    //        Console.Title = "Безопасное извлечение диска";

    //        while (true)
    //        {
    //            UpdateDriveList();

    //            if (_removableDrives.Count == 0)
    //            {
    //                Console.WriteLine("Не подключено ни одного флеш-диска.");
    //                Console.WriteLine("Нажмите любую клавишу для выхода...");
    //                Console.ReadKey(); // Ожидание нажатия клавиши перед выходом
    //                return;
    //            }

    //            DisplayDriveOptions();

    //            int choice = GetUserChoice();

    //            if (choice == 0)
    //            {
    //                EjectAllDrives();
    //            }
    //            else if (choice > 0 && choice <= _removableDrives.Count)
    //            {
    //                var drive = _removableDrives[choice - 1];
    //                EjectDrive(drive);
    //            }
    //            else
    //            {
    //                Console.WriteLine("Некорректный выбор.");
    //            }
    //        }
    //    }

    //    // Обновляет список подключенных съемных дисков
    //    private void UpdateDriveList()
    //    {
    //        _removableDrives.Clear();
    //        foreach (var drive in DriveInfo.GetDrives())
    //        {
    //            if (drive.DriveType == DriveType.Removable && drive.IsReady)
    //            {
    //                _removableDrives.Add(drive);
    //            }
    //        }
    //    }

    //    // Отображает список доступных для извлечения дисков с детальной информацией
    //    private void DisplayDriveOptions()
    //    {
    //        Console.WriteLine($"БЕЗОПАСНОЕ ИЗВЛЕЧЕНИЕ ФЛЕШКИ\n");
    //        Console.WriteLine("Обнаружены следующие флеш-диски:");
    //        Console.WriteLine("--------------------------------");
    //        for (int i = 0; i < _removableDrives.Count; i++)
    //        {
    //            var drive = _removableDrives[i];
    //            //string driveType = GetDriveType(drive.Name);
    //            Console.Write($"{i + 1}: ");
    //            //Console.WriteLine($"\tТип диска: {driveType}");
    //            Console.Write($"{drive.Name} ");
    //            Console.Write($"[{drive.VolumeLabel}] ");
    //            Console.WriteLine($"{drive.TotalSize / (1024 * 1024 * 1024)} GB");
    //            Console.WriteLine("--------------------------------");
    //        }

    //        Console.WriteLine("Введите номер флеш-диска для извлечения или 0 для извлечения всех:");
    //    }

    //    // Получение выбора пользователя
    //    private int GetUserChoice()
    //    {
    //        int choice;
    //        while (!int.TryParse(Console.ReadLine(), out choice))
    //        {
    //            Console.WriteLine("Некорректный ввод, попробуйте снова.");
    //        }
    //        return choice;
    //    }

    //    // Безопасное извлечение одного диска
    //    private void EjectDrive(DriveInfo drive)
    //    {
    //        string devicePath = @"\\.\" + drive.Name.TrimEnd('\\');
    //        IntPtr handle = CreateFile(devicePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

    //        if (handle == IntPtr.Zero)
    //        {
    //            Console.WriteLine($"Не удалось получить доступ к {drive.Name}. Ошибка: {Marshal.GetLastWin32Error()}");
    //            return;
    //        }

    //        uint bytesReturned = 0;
    //        bool result = DeviceIoControl(handle, IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);

    //        if (result)
    //        {
    //            Console.WriteLine($"Флеш-диск {drive.Name} - {drive.VolumeLabel} успешно извлечен.");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Не удалось извлечь флеш-диск {drive.Name}. Ошибка: {Marshal.GetLastWin32Error()}");
    //        }

    //        CloseHandle(handle);
    //    }

    //    // Извлечение всех дисков
    //    private void EjectAllDrives()
    //    {
    //        foreach (var drive in _removableDrives)
    //        {
    //            EjectDrive(drive);
    //        }
    //    }

    //    // Получаем тип диска через WMI
    //    private string GetDriveType(string driveLetter)
    //    {
    //        string query = $"SELECT * FROM Win32_DiskDrive WHERE Index IN " +
    //                       $"(SELECT DiskIndex FROM Win32_DiskPartition WHERE DeviceID IN " +
    //                       $"(SELECT Antecedent FROM Win32_LogicalDiskToPartition WHERE Dependent LIKE '%{driveLetter.TrimEnd('\\')}%')))";
    //        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
    //        {
    //            foreach (ManagementObject obj in searcher.Get())
    //            {
    //                var mediaType = obj["MediaType"]?.ToString();
    //                if (!string.IsNullOrEmpty(mediaType))
    //                {
    //                    return mediaType;
    //                }
    //            }
    //        }
    //        return "Неизвестный тип диска";
    //    }

    //    // Импортируем необходимые функции для работы с Windows API
    //    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    //    private static extern IntPtr CreateFile(
    //        string lpFileName,
    //        uint dwDesiredAccess,
    //        uint dwShareMode,
    //        IntPtr lpSecurityAttributes,
    //        uint dwCreationDisposition,
    //        uint dwFlagsAndAttributes,
    //        IntPtr hTemplateFile);

    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool DeviceIoControl(
    //        IntPtr hDevice,
    //        uint dwIoControlCode,
    //        IntPtr lpInBuffer,
    //        uint nInBufferSize,
    //        IntPtr lpOutBuffer,
    //        uint nOutBufferSize,
    //        ref uint lpBytesReturned,
    //        IntPtr lpOverlapped);

    //    [DllImport("kernel32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool CloseHandle(IntPtr hObject);

    //    private const uint IOCTL_STORAGE_EJECT_MEDIA = 0x2D4808;
    //    private const uint FILE_SHARE_READ = 0x00000001;
    //    private const uint FILE_SHARE_WRITE = 0x00000002;
    //    private const uint OPEN_EXISTING = 3;
    //    private const uint GENERIC_READ = 0x80000000;
    //    private const uint GENERIC_WRITE = 0x40000000;
    //}

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        DriveRemover remover = new DriveRemover();
    //        remover.Start();
    //    }
    //}
    #endregion

    #region HandMade
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        bool isPressedEnter = false;
    //        string line = "";

    //        while (isPressedEnter == false)
    //        {
    //            ConsoleKeyInfo pressedKey = Console.ReadKey();

    //            if (pressedKey.Key == ConsoleKey.F1)
    //            {
    //                Console.WriteLine($"Вызвана команда Справка");
    //                continue;
    //            }

    //            if (pressedKey.Key == ConsoleKey.Enter)
    //            {
    //                Console.WriteLine($"Введена команда {line}");
    //                isPressedEnter = true;
    //                continue;
    //            }

    //            line += pressedKey.KeyChar;
    //        }


    //        Console.ReadKey();
    //        //Configuration config = new Configuration();
    //        //config.Execute();

    //        //Remover remover = new Remover();
    //        //remover.Work();
    //    }
    //}

    //class Remover
    //{
    //    public void Work()
    //    {
    //        //const string HelpCommand = "?";

    //        const ConsoleKey HelpCommand = ConsoleKey.F1;

    //        bool isWork = true;
    //        bool isOnlyOneDrive = GetRemovableDrivers().Length == 1;
    //        bool haveManyDrives = GetRemovableDrivers().Length > 1;

    //        while (isWork)
    //        {
    //            DriveInfo[] removableDrivers = GetRemovableDrivers();

    //            if (removableDrivers.Length == 0)
    //            {
    //                Console.WriteLine($"Съемные диски не обнаружены");
    //                //Console.ReadKey();
    //                //return;
    //            }

    //            //ShowDrivesInfo(removableDrivers);

    //            //if(removableDrivers.Length == 1 && isOnlyOneDrive)
    //            //{
    //            //    Console.WriteLine($"Диск отключен");
    //            //}
    //            //else
    //            //{
    //            //    Console.WriteLine($"Какой диск отключить?");

    //            //}

    //            ConsoleKeyInfo presseKey = Console.ReadKey();

    //            switch (presseKey.Key)
    //            {
    //                case HelpCommand:
    //                    // в разработке
    //                    break;
    //            }

    //        }


    //    }

    //    private DriveInfo[] GetRemovableDrivers()
    //    {
    //        DriveInfo[] allDrivers = DriveInfo.GetDrives();

    //        DriveInfo[] removableDrivers = allDrivers.Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable).ToArray();

    //        return removableDrivers;
    //    }

    //    private static void ShowDrivesInfo(DriveInfo[] allDrivers)
    //    {
    //        char separator = '=';
    //        int borderLength = 22;
    //        int startPosition = 0;
    //        int letterCount = 2;
    //        int byteConversionFactor = 1024;

    //        Console.WriteLine(new string(separator, borderLength));

    //        for (int i = 0; i < allDrivers.Length; i++)
    //        {
    //            DriveInfo driveInfo = allDrivers[i];

    //            if (allDrivers[i].IsReady)
    //            {
    //                Console.Write($"{i + 1}. ");
    //                Console.Write($"{driveInfo.Name.Substring(startPosition, letterCount)} ");
    //                Console.Write($"[{driveInfo.VolumeLabel}] ");
    //                Console.WriteLine($"{driveInfo.TotalSize / (1024 * 1024 * 1024)} ГБ ");

    //                Console.WriteLine(new string(separator, borderLength));
    //            }
    //        }
    //    }
    //}

    //class Configuration
    //{
    //    private About _about = new About();

    //    public void Execute()
    //    {
    //        Console.Title = _about.Title;
    //    }
    //}

    //class About
    //{
    //    public About()
    //    {
    //        Title = "USB Remover";
    //        Version = "альфа";
    //        Author = "черинильный навеститель";
    //        Description = "Безопасное отключение usb диска";
    //        Link = "ссылка на GitHub";
    //    }

    //    public string Title { get; private set; }
    //    public string Version { get; private set; }
    //    public string Author { get; private set; }
    //    public string Description { get; private set; }
    //    public string Link { get; private set; }

    //    public void ShowInfo()
    //    {
    //        Console.WriteLine($"Название: {Title}");
    //        Console.WriteLine($"Версия: {Version}");
    //        Console.WriteLine($"Автор: {Author}");
    //    }
    //}
    #endregion

    #region gemini
    class Program
    {
        #region Description
        //Основная логика программы:

        //Получение подключенных съемных дисков с помощью метода GetRemovableDrives().

        //Программа проверяет, сколько съемных дисков подключено:
        //Если не найдено ни одного диска, программа выводит сообщение и завершает выполнение.
        //Если найден один диск, программа автоматически пытается его извлечь.
        //Если дисков несколько, программа выводит список и просит пользователя выбрать диск для извлечения или извлечь все диски.
        //Запрос у пользователя: если несколько дисков, программа выводит список подключенных флеш-дисков, предлагает выбрать, какой извлечь, или дает возможность извлечь все сразу.

        //Вывод ошибки при некорректном вводе: если пользователь вводит неправильное значение, программа выводит соответствующее сообщение.
        #endregion
        static void Main(string[] args)
        {
            Console.Title = "Безопасное удаление диска";
            DriveRemover remover = new DriveRemover();
            bool isExit = false;

            while (isExit == false)
            {
                Console.Clear();
                List<DriveInfo> removableDrives = remover.GetRemovableDrives();

                if (removableDrives.Count == 0)
                {
                    Console.WriteLine("Не подключено ни одного флеш-диска.");
                }
                else
                {
                    Console.WriteLine("Обнаружены следующие флеш-диски:");

                    for (int i = 0; i < removableDrives.Count; i++)
                    {
                        DriveInfo drive = removableDrives[i];
                        long usedSpace = drive.TotalSize - drive.TotalFreeSpace;
                        Console.WriteLine($"{i + 1}: {drive.Name} (Занято: {remover.GetFormattedSize(usedSpace)}, " +
                            $"Свободно: {remover.GetFormattedSize(drive.TotalFreeSpace)}, Всего: {remover.GetFormattedSize(drive.TotalSize)})");
                    }

                    Console.WriteLine("\nВведите номер диска для извлечения или 0 для всех.");
                }

                Console.WriteLine("\n[F5] - Обновить список | [Esc] - Выход");

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.F5)
                {
                    continue;
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    isExit = true;
                    continue;
                }

                if (char.IsDigit(key.KeyChar) == true && removableDrives.Count > 0)
                {
                    int choice = int.Parse(key.KeyChar.ToString());

                    if (choice == 0)
                    {
                        foreach (DriveInfo drive in removableDrives)
                        {
                            remover.SafelyRemoveDrive(drive.Name);
                        }
                    }
                    else if (choice > 0 && choice <= removableDrives.Count)
                    {
                        remover.SafelyRemoveDrive(removableDrives[choice - 1].Name);
                    }
                    else
                    {
                        Console.WriteLine("Некорректный выбор.");
                    }

                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey(true);
                }
            }
        }
    }

    class DriveRemover
    {
        #region Import_Win_API
        //Функция CreateFile из kernel32.dll используется для получения дескриптора файла или устройства.
        //В данном случае используется для открытия флеш-диска как устройства,
        //чтобы потом с ним можно было взаимодействовать
        //(например, отправлять команду на извлечение).
        #endregion
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        #region DeviceIoControl
        //Функция DeviceIoControl — это универсальный метод для управления устройствами, такими как диски.
        //С помощью этого метода отправляется команда на безопасное извлечение флеш-диска.
        //
        //Параметры:
        //hDevice — дескриптор устройства (возвращенный CreateFile).
        //dwIoControlCode — код команды, например, IOCTL_STORAGE_EJECT_MEDIA для извлечения диска.
        //lpInBuffer и lpOutBuffer — буферы для передачи данных (здесь они не нужны).
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped
            );

        #region CloseHandle
        //Функция CloseHandle закрывает дескриптор устройства после завершения работы с ним,
        //освобождая системные ресурсы.
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        public List<DriveInfo> GetRemovableDrives()
        {
            List<DriveInfo> removableDrives = new List<DriveInfo>();

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    removableDrives.Add(drive);
                }
            }

            return removableDrives;
        }

        public string GetFormattedSize(long bytes)
        {
            const long Kilobyte = 1024;
            const long Megabyte = Kilobyte * 1024;
            const long Gigabyte = Megabyte * 1024;

            if (bytes >= Gigabyte)
            {
                return $"{(double)bytes / Gigabyte:F2} ГБ";
            }

            if (bytes >= Megabyte)
            {
                return $"{(double)bytes / Megabyte:F2} МБ";
            }

            if (bytes >= Kilobyte)
            {
                return $"{(double)bytes / Kilobyte:F2} КБ";
            }

            return $"{bytes} Байт";
        }

        public void SafelyRemoveDrive(string driveLetter)
        {
            #region CONST_WINDOWS_API
            // Константы для Windows API
            //константы используются для взаимодействия с функциями Windows API. Например:

            //IOCTL_STORAGE_EJECT_MEDIA — код управления, который отправляется устройству для его безопасного извлечения.
            //FILE_SHARE_READ и FILE_SHARE_WRITE — указывают, что другие процессы могут иметь доступ к устройству на чтение и запись.
            //OPEN_EXISTING — используется при открытии существующего устройства(флеш - диска).
            //GENERIC_READ и GENERIC_WRITE — права доступа к устройству(чтение и запись). 
            #endregion
            const uint CodeEject = 0x2D4808;
            const uint FileShareRead = 0x00000001;
            const uint FileShareWrite = 0x00000002;
            const uint OpenExisting = 3;
            const uint GenericRead = 0x80000000;
            const uint GenericWrite = 0x40000000;

            string devicePath = @"\\.\" + driveLetter.TrimEnd('\\');
            IntPtr handle = CreateFile(devicePath, GenericRead | GenericWrite, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);

            if (handle == IntPtr.Zero)
            {
                Console.WriteLine($"Не удалось получить доступ к {driveLetter}. Ошибка: {Marshal.GetLastWin32Error()}");
                return;
            }

            uint bytesReturned = 0;
            bool result = DeviceIoControl(handle, CodeEject, IntPtr.Zero, 0, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);

            if (result == true)
            {
                Console.WriteLine($"Флеш-диск {driveLetter} успешно извлечен.");
            }
            else
            {
                Console.WriteLine($"Не удалось извлечь флеш-диск {driveLetter}. Ошибка: {Marshal.GetLastWin32Error()}");
            }

            CloseHandle(handle);
        }
    }
    #endregion
}
