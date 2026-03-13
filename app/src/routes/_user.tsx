import { createFileRoute, Link, Outlet, redirect, useLocation } from "@tanstack/react-router";
import {
  ArrowLeftRight,
  Bell,
  ChevronRight,
  CreditCard,
  FileText,
  HelpCircle,
  LayoutDashboard,
  LineChart,
  Lock,
  LogOut,
  Menu,
  PiggyBank,
  Settings,
  Shield,
  User,
} from "lucide-react";
import { useState } from "react";

import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { cn } from "@/libraries/utils";
import { api } from "@/services/api";

export const Route = createFileRoute("/_user")({
  beforeLoad: async () => {
    const user = await api.users.me();
    if (user === undefined) {
      throw redirect({ to: "/register" });
    }
  },
  component: DashboardComponent,
});

const NAV_ITEMS = [
  {
    label: "Overview",
    icon: LayoutDashboard,
    href: "/",
    badge: null,
  },
  {
    label: "Transactions",
    icon: ArrowLeftRight,
    href: "/transactions",
    badge: null,
  },
  {
    label: "Cards",
    icon: CreditCard,
    href: "/cards",
    badge: "2",
  },
  {
    label: "Accounts",
    icon: PiggyBank,
    href: "/accounts",
    badge: null,
  },
  {
    label: "Investments",
    icon: LineChart,
    href: "/investments",
    badge: null,
  },
  {
    label: "Statements",
    icon: FileText,
    href: "/statements",
    badge: null,
  },
];

const BOTTOM_NAV = [
  { label: "Settings", icon: Settings, href: "/settings" },
  { label: "Help & Support", icon: HelpCircle, href: "/support" },
];

function NavItem({ item, collapsed, active }: { item: (typeof NAV_ITEMS)[0]; collapsed: boolean; active: boolean }) {
  const Icon = item.icon;

  const inner = (
    <Link
      to={item.href}
      className={cn(
        "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-all duration-150",
        "hover:bg-accent hover:text-accent-foreground",
        active
          ? "bg-primary text-primary-foreground shadow-sm hover:bg-primary hover:text-primary-foreground"
          : "text-muted-foreground",
      )}
    >
      <Icon className="h-4 w-4 shrink-0" />
      {!collapsed && (
        <>
          <span className="flex-1">{item.label}</span>
          {item.badge && (
            <Badge variant={active ? "secondary" : "outline"} className="h-5 min-w-5 justify-center px-1 text-[10px]">
              {item.badge}
            </Badge>
          )}
        </>
      )}
    </Link>
  );

  if (collapsed) {
    return (
      <TooltipProvider delayDuration={0}>
        <Tooltip>
          <TooltipTrigger asChild>{inner}</TooltipTrigger>
          <TooltipContent side="right" className="flex items-center gap-2">
            {item.label}
            {item.badge && <Badge variant="secondary">{item.badge}</Badge>}
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    );
  }

  return inner;
}

function Sidebar({ collapsed, onToggle }: { collapsed: boolean; onToggle: () => void }) {
  const location = useLocation();

  return (
    <aside
      className={cn("flex h-full flex-col border-r bg-card transition-all duration-300", collapsed ? "w-15" : "w-55")}
    >
      {/* Logo */}
      <div
        className={cn("flex h-16 items-center border-b px-3", collapsed ? "justify-center" : "justify-between px-4")}
      >
        {!collapsed && (
          <div className="flex items-center gap-2">
            <div className="flex h-7 w-7 items-center justify-center rounded-md bg-primary">
              <Shield className="h-4 w-4 text-primary-foreground" />
            </div>
            <span className="text-base font-semibold tracking-tight">Arcadia</span>
          </div>
        )}
        {collapsed && (
          <div className="flex h-7 w-7 items-center justify-center rounded-md bg-primary">
            <Shield className="h-4 w-4 text-primary-foreground" />
          </div>
        )}
        <Button
          variant="ghost"
          size="icon"
          className={cn("h-7 w-7 shrink-0", collapsed && "hidden")}
          onClick={onToggle}
        >
          <ChevronRight className={cn("h-4 w-4 transition-transform", !collapsed && "rotate-180")} />
        </Button>
      </div>

      <ScrollArea className="flex-1 px-2 py-3">
        {/* Account summary pill */}
        {!collapsed && (
          <div className="mb-4 rounded-lg border bg-muted/40 p-3">
            <p className="text-[11px] font-medium text-muted-foreground uppercase tracking-wider mb-1">Total Balance</p>
            <p className="text-xl font-semibold tracking-tight">$48,291.04</p>
            <p className="text-[11px] text-emerald-600 font-medium mt-0.5">↑ 2.4% this month</p>
          </div>
        )}

        <nav className="space-y-0.5">
          {NAV_ITEMS.map((item) => (
            <NavItem key={item.href} item={item} collapsed={collapsed} active={location.pathname === item.href} />
          ))}
        </nav>

        <Separator className="my-3" />

        <nav className="space-y-0.5">
          {BOTTOM_NAV.map((item) => (
            <NavItem key={item.href} item={item} collapsed={collapsed} active={location.pathname === item.href} />
          ))}
        </nav>
      </ScrollArea>

      {/* User section */}
      <div className={cn("border-t p-2", collapsed ? "px-2" : "px-3 py-3")}>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              variant="ghost"
              className={cn("w-full justify-start gap-2 px-2 h-auto py-1.5", collapsed && "justify-center px-0")}
            >
              <Avatar className="h-7 w-7 shrink-0">
                <AvatarImage src="/avatar.png" />
                <AvatarFallback className="text-xs bg-primary/10 text-primary font-semibold">JD</AvatarFallback>
              </Avatar>
              {!collapsed && (
                <div className="flex flex-col items-start text-left min-w-0">
                  <span className="text-xs font-medium truncate">Jane Doe</span>
                  <span className="text-[10px] text-muted-foreground truncate">Personal Account</span>
                </div>
              )}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent side="top" align="start" className="w-52">
            <DropdownMenuLabel className="font-normal">
              <div className="flex flex-col gap-0.5">
                <span className="font-semibold text-sm">Jane Doe</span>
                <span className="text-xs text-muted-foreground">jane@example.com</span>
              </div>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem>
              <User className="mr-2 h-4 w-4" />
              Profile
            </DropdownMenuItem>
            <DropdownMenuItem>
              <Lock className="mr-2 h-4 w-4" />
              Security
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="text-destructive focus:text-destructive">
              <LogOut className="mr-2 h-4 w-4" />
              Sign out
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </aside>
  );
}

function TopBar({ title }: { title: string }) {
  return (
    <header className="flex h-16 items-center gap-4 border-b bg-background/95 backdrop-blur-sm px-6 sticky top-0 z-10">
      {/* Mobile nav trigger */}
      <Sheet>
        <SheetTrigger asChild>
          <Button variant="ghost" size="icon" className="md:hidden shrink-0">
            <Menu className="h-5 w-5" />
          </Button>
        </SheetTrigger>
        <SheetContent side="left" className="p-0 w-55">
          <Sidebar collapsed={false} onToggle={() => {}} />
        </SheetContent>
      </Sheet>

      <div className="flex-1 min-w-0">
        <h1 className="text-base font-semibold truncate">{title}</h1>
        <p className="text-xs text-muted-foreground hidden sm:block">
          {new Date().toLocaleDateString("en-US", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </p>
      </div>

      <div className="flex items-center gap-2">
        {/* Security badge */}
        <Badge
          variant="outline"
          className="hidden sm:flex items-center gap-1 text-emerald-600 border-emerald-200 bg-emerald-50 dark:bg-emerald-950 dark:border-emerald-800"
        >
          <Shield className="h-3 w-3" />
          <span className="text-[11px]">Secured</span>
        </Badge>

        {/* Notifications */}
        <TooltipProvider>
          <Tooltip>
            <TooltipTrigger asChild>
              <Button variant="ghost" size="icon" className="relative">
                <Bell className="h-4 w-4" />
                <span className="absolute top-1.5 right-1.5 h-2 w-2 rounded-full bg-destructive" />
              </Button>
            </TooltipTrigger>
            <TooltipContent>Notifications</TooltipContent>
          </Tooltip>
        </TooltipProvider>
      </div>
    </header>
  );
}

function DashboardComponent() {
  const [collapsed, setCollapsed] = useState(false);
  const location = useLocation();

  const currentNav = [...NAV_ITEMS, ...BOTTOM_NAV].find((item) => item.href === location.pathname);
  const pageTitle = currentNav?.label ?? "Dashboard";

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      {/* Desktop sidebar */}
      <div className="hidden md:flex">
        <Sidebar collapsed={collapsed} onToggle={() => setCollapsed((c) => !c)} />
      </div>

      {/* Main content area */}
      <div className="flex flex-1 flex-col min-w-0 overflow-hidden">
        <TopBar title={pageTitle} />

        <main className="flex-1 overflow-y-auto">
          <div className="container max-w-6xl mx-auto p-6">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
