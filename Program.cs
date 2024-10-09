using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SafeRemover
{
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

            List<string> removableDrives = remover.GetRemovableDrives();

            if (removableDrives.Count == 0)
            {
                Console.WriteLine("Не подключено ни одного флеш-диска.");
                return;
            }
            else if (removableDrives.Count == 1)
            {
                Console.WriteLine($"Обнаружен флеш-диск: {removableDrives[0]}");
                remover.SafelyRemoveDrive(removableDrives[0]);
            }
            else
            {
                Console.WriteLine("Обнаружены следующие флеш-диски:");

                for (int i = 0; i < removableDrives.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {removableDrives[i]}");
                }

                Console.WriteLine("Введите номер флеш-диска для извлечения или 0 для извлечения всех:");
                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    foreach (var drive in removableDrives)
                    {
                        remover.SafelyRemoveDrive(drive);
                    }
                }
                else if (choice > 0 && choice <= removableDrives.Count)
                {
                    remover.SafelyRemoveDrive(removableDrives[choice - 1]);
                }
                else
                {
                    Console.WriteLine("Некорректный выбор.");
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

        public List<string> GetRemovableDrives()
        {
            List<string> removableDrives = new List<string>();

            var allDrives = DriveInfo.GetDrives();

            foreach (var drive in allDrives)
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    removableDrives.Add(drive.Name);
                }
            }

            return removableDrives;
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

            if (result)
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
}
