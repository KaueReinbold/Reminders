import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

/// Design tokens from docs/design/reminders-redesign/design-system.md.
/// These are the only approved values; do not invent new ones.
abstract final class AppColors {
  static const canvas = Color(0xFFF3F0E9);
  static const panel = Color(0xFFEFEBE1);
  static const surface = Color(0xFFFBFAF7);
  static const inputFill = Color(0xFFF7F5F0);
  static const ink = Color(0xFF1B1A17);
  static const bodyMuted = Color(0xFF6E6759);
  static const faint = Color(0xFF8C8577);
  static const disabled = Color(0xFF9A9385);
  static const borderPage = Color(0xFFE0DACD);
  static const borderCard = Color(0xFFE7E2D6);
  static const borderInput = Color(0xFFDDD7C9);
  static const borderHover = Color(0xFFCFC7B5);
  static const borderHoverStrong = Color(0xFFC6BDA9);
  static const accent = Color(0xFFB4451F);
  static const accentHover = Color(0xFF8E3415);
  static const accentTint = Color(0xFFF6E4DC);
  static const success = Color(0xFF4C6B4F);
  static const successTint = Color(0xFFEDF1EC);
  static const scrim = Color(0x611B1A17); // rgba(27,26,23,0.38)
  static const pillNeutralBg = Color(0xFFF0EDE4);
}

abstract final class AppRadii {
  static const pill = 999.0;
  static const card = 14.0;
  static const modal = 18.0;
  static const input = 10.0;
  static const nav = 10.0;
}

abstract final class AppSpace {
  static const gapCards = 10.0;
  static const gapSections = 30.0;
  static const gapFields = 18.0;
  static const padModal = 28.0;
  static const padCard = EdgeInsets.symmetric(vertical: 16, horizontal: 18);
}

ThemeData buildAppTheme() {
  final base = ThemeData(
    useMaterial3: true,
    scaffoldBackgroundColor: AppColors.canvas,
    colorScheme: ColorScheme.fromSeed(
      seedColor: AppColors.accent,
      surface: AppColors.surface,
      error: AppColors.accent,
    ),
  );

  final textTheme = GoogleFonts.dmSansTextTheme(base.textTheme).apply(
    bodyColor: AppColors.ink,
    displayColor: AppColors.ink,
  );

  return base.copyWith(
    textTheme: textTheme.copyWith(
      // Display type uses Instrument Serif per the design system.
      headlineLarge: GoogleFonts.instrumentSerif(
        fontSize: 30,
        fontWeight: FontWeight.w400,
        color: AppColors.ink,
      ),
      headlineMedium: GoogleFonts.instrumentSerif(
        fontSize: 22,
        fontWeight: FontWeight.w400,
        color: AppColors.ink,
      ),
      bodyMedium: textTheme.bodyMedium?.copyWith(fontSize: 15),
    ),
  );
}
