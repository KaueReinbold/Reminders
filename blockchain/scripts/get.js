const { ethers } = require("hardhat");

async function main() {
    // Get contract address from command line argument
    const contractAddress = '0x38cF23C52Bb4B13F051Aec09580a2dE845a7FA35';

    const Reminders = await ethers.getContractFactory("Reminders");
    const reminders = Reminders.attach(contractAddress);

    // Get the reminder at index 0
    const reminder = await reminders.getReminder(0);
    console.log("Reminder at index 0:", reminder);
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
});