import { Component, inject, Input, input, output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  accountService = inject(AccountService)
  usersFromHomeComponent = input.required<any>()
  cancelRegister = output<boolean>()
  model: any = {}

  register() {
    this.accountService.register(this.model).subscribe();
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
