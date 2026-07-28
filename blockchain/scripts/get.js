const { ethers } = require("hardhat");

async function main() {
    const contractAddress = process.env.BLOCKCHAIN_CONTRACT_ADDRESS || '0xf204a4Ef082f5c04bB89F7D5E6568B796096735a';

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