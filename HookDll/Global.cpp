#include "Global.h"

namespace {
	bool _isInstall = false;
	//WNDPROC originalWndProc;
	HGLOBAL originalWndProc;

	std::string toHex(int n)
	{
		std::string s = "";
		if (n == 0)
			s = "0";

		while (n != 0)
		{
			if (n % 16 >9)
				s += n % 16 - 10 + 'A';
			else
				s += n % 16 + '0';
			n = n / 16;
		}

		reverse(s.begin(), s.end());//反转
		return s + "H";
	}

	HWND currentProcessMainWindow;

	BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam)
	{
		DWORD dwCurProcessId = *((DWORD*)lParam);
		DWORD dwProcessId = 0;

		GetWindowThreadProcessId(hwnd, &dwProcessId);
		if (dwProcessId == dwCurProcessId && GetParent(hwnd) == NULL)
		{
			currentProcessMainWindow = hwnd;
			//*((HWND *)lParam) = hwnd;
			return FALSE;
		}
		return TRUE;
	}

	HWND GetMainWindow()
	{
		DWORD dwCurrentProcessId = GetCurrentProcessId();
		if (!EnumWindows(EnumWindowsProc, (LPARAM)&dwCurrentProcessId))
		{
			//return (HWND)dwCurrentProcessId;
			return currentProcessMainWindow;
		}
		return nullptr;
	}

	LRESULT CALLBACK wndProc(_In_ HWND hWnd, _In_ UINT msg, _In_ WPARAM wParam,
							_In_ LPARAM lParam)
	{
		Global::log(std::to_string(msg));
#if WM_APP != 0x8000
#error 修改传递参数为正确数值
#endif // WM_App
		switch (msg) {
			case WM_NCACTIVATE:
			case WM_ACTIVATE:
			case WM_ACTIVATEAPP: {
				if (wParam == WA_INACTIVE) {
					Global::log("hook");
					return CallWindowProc((WNDPROC)originalWndProc, hWnd, msg, WA_ACTIVE, lParam);
				}
			} break;
			case 0x801: {
				if (Global::uninstall()) {
					return 1;
				}
				else {
					return 2;
				}
			} break;
		}
		return CallWindowProc((WNDPROC)originalWndProc, hWnd, msg, wParam, lParam);
	}

}

bool Global::isInstall()
{
	return _isInstall;
}

void Global::log(std::string s)
{
#ifdef _DEBUG
	const static std::string logFilePath = "%TEMP%\\HookWindowActivity.log";
	std::ofstream stream;
	stream.open(logFilePath, std::ios::app);
	if (!stream) {
		return;
	}
	std::time_t time = std::time(nullptr);
	struct tm timeinfo;
	localtime_s(&timeinfo, &time);
	stream << std::put_time(&timeinfo, "%Y-%m-%d %H.%M.%S") << "    " << s << "\n";
	stream.close();
#endif // _DEBUG
}

void Global::install()
{
	log("start install");
	originalWndProc = (HGLOBAL)SetWindowLongPtr(GetMainWindow(), GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(wndProc));
	if (originalWndProc == 0) {
		log("init fail");
		log(std::string("SetWindowLong error, error code:") + std::to_string((unsigned long)GetLastError()));
	}
	else {
		log("init success");
		_isInstall = true;
	}
}

void Global::installIfNotInstall()
{
	if (!_isInstall) {
		install();
	}
}

bool Global::uninstall()
{
	log("start uninstall");
	if (_isInstall) {
		HGLOBAL result = (HGLOBAL)SetWindowLongPtr(GetMainWindow(), GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(originalWndProc));
		if (result == 0) {
			log("uninstall fail");
			return false;
		}
		else {
			log("uninstall finished");
			_isInstall = false;
			originalWndProc = 0;
			return true;
		}
	}
	else {
		return true;
	}
}
