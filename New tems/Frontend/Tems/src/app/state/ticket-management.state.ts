import { Injectable, signal, computed } from '@angular/core';
import { TicketType } from '../models/ticket/ticket-type.model';
import { Ticket } from '../models/ticket/ticket.model';

export interface TicketManagementState {
  ticketTypes: TicketType[];
  tickets: Ticket[];
  ticketTypesLoading: boolean;
  ticketsLoading: boolean;
  ticketTypesLoaded: boolean;
  ticketsLoaded: boolean;
  ticketTypesError: string | null;
  ticketsError: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class TicketManagementStateService {
  private state = signal<TicketManagementState>({
    ticketTypes: [],
    tickets: [],
    ticketTypesLoading: false,
    ticketsLoading: false,
    ticketTypesLoaded: false,
    ticketsLoaded: false,
    ticketTypesError: null,
    ticketsError: null
  });

  // Read-only signals for components
  ticketTypes = computed(() => this.state().ticketTypes);
  tickets = computed(() => this.state().tickets);
  ticketTypesLoading = computed(() => this.state().ticketTypesLoading);
  ticketsLoading = computed(() => this.state().ticketsLoading);
  ticketTypesLoaded = computed(() => this.state().ticketTypesLoaded);
  ticketsLoaded = computed(() => this.state().ticketsLoaded);
  ticketTypesError = computed(() => this.state().ticketTypesError);
  ticketsError = computed(() => this.state().ticketsError);

  // Ticket Types actions
  setTicketTypesLoading(loading: boolean): void {
    this.state.update(state => ({
      ...state,
      ticketTypesLoading: loading,
      ticketTypesError: loading ? null : state.ticketTypesError
    }));
  }

  setTicketTypes(ticketTypes: TicketType[]): void {
    this.state.update(state => ({
      ...state,
      ticketTypes,
      ticketTypesLoading: false,
      ticketTypesLoaded: true,
      ticketTypesError: null
    }));
  }

  setTicketTypesError(error: string): void {
    this.state.update(state => ({
      ...state,
      ticketTypesLoading: false,
      ticketTypesError: error
    }));
  }

  addTicketType(ticketType: TicketType): void {
    this.state.update(state => ({
      ...state,
      ticketTypes: [...state.ticketTypes, ticketType]
    }));
  }

  updateTicketType(id: string, updates: Partial<TicketType>): void {
    this.state.update(state => ({
      ...state,
      ticketTypes: state.ticketTypes.map(tt =>
        tt.ticketTypeId === id ? { ...tt, ...updates } : tt
      )
    }));
  }

  removeTicketType(id: string): void {
    this.state.update(state => ({
      ...state,
      ticketTypes: state.ticketTypes.filter(tt => tt.ticketTypeId !== id)
    }));
  }

  invalidateTicketTypes(): void {
    this.state.update(state => ({
      ...state,
      ticketTypesLoaded: false
    }));
  }

  // Tickets actions
  setTicketsLoading(loading: boolean): void {
    this.state.update(state => ({
      ...state,
      ticketsLoading: loading,
      ticketsError: loading ? null : state.ticketsError
    }));
  }

  setTickets(tickets: Ticket[]): void {
    this.state.update(state => ({
      ...state,
      tickets,
      ticketsLoading: false,
      ticketsLoaded: true,
      ticketsError: null
    }));
  }

  setTicketsError(error: string): void {
    this.state.update(state => ({
      ...state,
      ticketsLoading: false,
      ticketsError: error
    }));
  }

  addTicket(ticket: Ticket): void {
    this.state.update(state => ({
      ...state,
      tickets: [...state.tickets, ticket]
    }));
  }

  updateTicket(id: string, updates: Partial<Ticket>): void {
    this.state.update(state => ({
      ...state,
      tickets: state.tickets.map(t =>
        t.ticketId === id ? { ...t, ...updates } : t
      )
    }));
  }

  removeTicket(id: string): void {
    this.state.update(state => ({
      ...state,
      tickets: state.tickets.filter(t => t.ticketId !== id)
    }));
  }

  invalidateTickets(): void {
    this.state.update(state => ({
      ...state,
      ticketsLoaded: false
    }));
  }

  // Clear all state
  reset(): void {
    this.state.set({
      ticketTypes: [],
      tickets: [],
      ticketTypesLoading: false,
      ticketsLoading: false,
      ticketTypesLoaded: false,
      ticketsLoaded: false,
      ticketTypesError: null,
      ticketsError: null
    });
  }
}
