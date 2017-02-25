#pragma once

#include "stdafx.h"

class Global
{
public:
	static bool isInstall();
	static void log(std::string s);
	static void install();
	static void installIfNotInstall();
	static bool uninstall();
private:
	Global() {
	}
};

