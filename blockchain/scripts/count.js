const { ethers } = require("hardhat");

async function main() {
    const contractAddress = process.env.BLOCKCHAIN_CONTRACT_ADDRESS || '0xf204a4Ef082f5c04bB89F7D5E6568B796096735a';
    const reminder = 'test-reminder';

    const Reminders = await ethers.getContractFactory("Reminders");
    const reminders = Reminders.attach(contractAddress);

    // Call the getReminderCount function
    const count = await reminders.getReminderCount();

    console.log("Reminder count:", count);
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
});
