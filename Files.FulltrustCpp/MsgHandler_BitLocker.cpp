#include "pch.h"
#include "MsgHandler_BitLocker.h"
#include "NativeMethods.h"
#include <string>
#include <sstream>

bool MsgHandler_BitLocker::Unlock(LPCWSTR volume, LPCWSTR password)
{
	using namespace::std;

	wstringstream strStream;
	strStream
		<< L"-command \"$SecureString = ConvertTo-SecureString '"
		<< std::wstring(password)
		<< L"' -AsPlainText -Force; Unlock-BitLocker -MountPoint '"
		<< std::wstring(volume)
		<< L"' -Password $SecureString\"";

	wstring command = strStream.str();

	return RunPowershellCommand(command);
}

bool MsgHandler_BitLocker::Lock(LPCWSTR volume, LPCWSTR password)
{
	using namespace::std;

	wstringstream strStream;
	strStream
		<< L"-command \"$SecureString = ConvertTo-SecureString '"
		<< std::wstring(password)
		<< L"' -AsPlainText -Force; Lock-BitLocker -MountPoint '"
		<< std::wstring(volume)
		<< L"' -Password $SecureString\"";

	wstring command = strStream.str();

	return RunPowershellCommand(command);
}

IAsyncOperation<bool> MsgHandler_BitLocker::ParseArgumentsAsync(const AppServiceManager& manager, const AppServiceRequestReceivedEventArgs& args)
{
	using namespace::winrt;

	if (args.Request().Message().HasKey(L"Arguments"))
	{
		hstring arguments = args.Request().Message().Lookup(L"Arguments").as<hstring>();

		if (arguments == L"Bitlocker")
		{
			hstring action = args.Request().Message().Lookup(L"action").as<hstring>();
			hstring filepath = args.Request().Message().Lookup(L"drive").as<hstring>();
			hstring password = args.Request().Message().Lookup(L"password").as<hstring>();

			if (action == L"Unlock")
			{
				if (Unlock(filepath.c_str(), password.c_str()))
				{
					ValueSet response;
					response.Insert(L"Bitlocker", winrt::box_value(L"Unlock"));
					co_await args.Request().SendResponseAsync(response);
				}

				co_return TRUE;
			}
			else if (action == L"Lock")
			{
				if (Lock(filepath.c_str(), password.c_str()))
				{
					ValueSet response;
					response.Insert(L"Bitlocker", winrt::box_value(L"Lock"));
					co_await args.Request().SendResponseAsync(response);
				}

				co_return TRUE;
			}
		}
	}

	co_return FALSE;
}